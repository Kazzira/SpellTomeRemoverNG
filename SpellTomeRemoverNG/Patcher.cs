using LanguageExt;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using static LanguageExt.Prelude;

namespace SpellTomeRemoverNG;

public class Patcher(
    IPatcherState<ISkyrimMod, ISkyrimModGetter> PatcherState,
    IPatcherLogger PatcherLogger
)
{
    private readonly IPatcherState<ISkyrimMod, ISkyrimModGetter> State = PatcherState;
    private readonly IPatcherLogger Logger = PatcherLogger;

    private PatcherStats Stats = new();

    private bool CanContainerBeSkipped(IContainerGetter container)
    {
        // Skip containers from blacklisted mods.
        if (IsContainerBlacklisted(container))
        {
            ++Stats.ContainerRemover.ContainersSkipped;
            return true;
        }

        return false;
    }

    private bool CanContainerItemBeSkipped(IContainerItemGetter containerItem)
    {
        // A book?
        if (!containerItem.Item.TryResolve<IBookGetter>(State.LinkCache, out var book)) return true;
        // A spell tome?
        if (book.Teaches is not IBookSpellGetter) return true;

        // Spell tome.

        // Skip spell tomes from blacklisted mods.
        if (IsBookBlacklisted(book))
        {
            ++Stats.ContainerRemover.SpellTomesSkipped;
            return true;
        }

        // Do not skip.
        return false;
    }

    private bool CanLeveledItemBeSkipped(ILeveledItemGetter leveledItem)
    {
        // Skip leveled lists from blacklisted mods.
        if (IsLeveledListBlacklisted(leveledItem))
        {
            ++Stats.LeveledItemRemover.LeveledListsSkipped;
            return true;
        }

        // Do not skip.
        return false;
    }

    private bool CanLeveledItemEntryBeSkipped(ILeveledItemEntryGetter entry)
    {
        // No entry data?
        if (entry.Data is null) return true;
        // Is this a book?        
        if (!State.LinkCache.TryResolve<IBookGetter>(entry.Data.Reference.FormKey, out var book)) return true;
        // Is the book a spell tome?
        if (book.Teaches is not IBookSpellGetter) return true;

        // Skip spell tomes from blacklisted mods.
        if (IsBookBlacklisted(book))
        {
            ++Stats.LeveledItemRemover.SpellTomesSkipped;
            return true;
        }

        // Do not skip.
        return false;
    }

    private bool CanPlacedSpellTomeBeSkipped(IPlacedObjectGetter placedObject)
    {
        // Item already disabled?
        if (placedObject.MajorRecordFlagsRaw == 0x0000_0800) return true;
        // A book?
        if (!placedObject.Base.TryResolve<IBookGetter>(State.LinkCache, out var placedBook)) return true;
        // A spell tome?
        if (placedBook.Teaches is not IBookSpellGetter) return true;

        // Placed item is from a blacklisted mod?
        if (IsPlacedItemBlacklisted(placedObject))
        {
            ++Stats.PlacedObjectRemover.SpellTomesSkipped;
            return true;
        }

        // book is from a blacklisted mod?
        if (IsBookBlacklisted(placedBook))
        {
            ++Stats.PlacedObjectRemover.SpellTomesSkipped;
            return true;
        }

        // Do not skip.
        return false;
    }

    /// <summary>
    /// Returns true if the object is blacklisted.
    /// </summary>
    /// <param name="formKeyGetter">The object to check.</param>
    /// <param name="predicate">Takes a PluginBlacklistEntry and returns a bool.</param>
    /// <returns>true if object is blacklisted, false otherwise.</returns>
    private static bool IsObjectBlacklisted(
        IFormKeyGetter formKeyGetter,
        Func<PluginBlacklistEntry?, bool> predicate
    )
    {
        return predicate(
            Settings.Instance.Value.PluginBlackList.FirstOrDefault(
                entry => entry.PluginFileName == formKeyGetter.FormKey.ModKey.FileName
            )
        );
    }

    private static bool IsContainerBlacklisted(IContainerGetter container) =>
        IsObjectBlacklisted(container, entry => entry?.BlacklistContainers ?? false);

    private static bool IsLeveledListBlacklisted(ILeveledItemGetter leveledItem) =>
        IsObjectBlacklisted(leveledItem, entry => entry?.BlacklistLeveledLists ?? false);

    private static bool IsPlacedItemBlacklisted(IPlacedObjectGetter placedObject) =>
        IsObjectBlacklisted(placedObject, entry => entry?.BlacklistPlacedObjects ?? false);

    private static bool IsBookBlacklisted(IBookGetter book) =>
        IsObjectBlacklisted(book, entry => entry?.BlacklistBooks ?? false);

    private Action<IContainer> HandleRemoveItemsFromContainer(List<int> itemIndicesToRemove)
    {
        return ctnr =>
        {
            ++Stats.ContainerRemover.ContainersModified;
            RemoveItemsFromContainer(ctnr, itemIndicesToRemove);
        };
    }

    private Action<ILeveledItem> HandleRemoveItemsFromLeveledList(List<int> itemIndicesToRemove)
    {
        return lvli =>
        {
            ++Stats.LeveledItemRemover.LeveledListsModified;
            RemoveItemsFromLeveledList(lvli, itemIndicesToRemove);
        };
    }

    private static void NoOp() { }

    private static void RemoveItemsFromContainer(IContainer container, List<int> itemIndicesToRemove)
    {
        if (container.Items is null) return;

        foreach (var index in itemIndicesToRemove.OrderByDescending(x => x))
        {
            container.Items.RemoveAt(index);
        }
    }

    private static void RemoveItemsFromLeveledList(ILeveledItem leveledItem, List<int> itemIndicesToRemove)
    {
        if (leveledItem.Entries is null) return;

        foreach (var index in itemIndicesToRemove.OrderByDescending(x => x))
        {
            leveledItem.Entries.RemoveAt(index);
        }
    }

    private void RemovePlacedSpellTomes()
    {
        Console.WriteLine("Removing placed spell tomes...");

        foreach (var placedObject in State.LoadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(State.LinkCache))

        {
            if (CanPlacedSpellTomeBeSkipped(placedObject.Record)) continue;

            // Remove the placed spell tome.
            IPlacedObject modifiedPlacedBook = placedObject.GetOrAddAsOverride(State.PatchMod);
            modifiedPlacedBook.MajorRecordFlagsRaw |= 0x0000_0800;
            ++Stats.PlacedObjectRemover.SpellTomesRemoved;
        }

        Logger.LogPlacedObjectRemoverStats(in Stats.PlacedObjectRemover);
    }

    private void RemoveSpellTomesFromContainers()
    {
        Console.WriteLine("Removing spell tomes from containers...");

        foreach (var container in State.LoadOrder.PriorityOrder.Container().WinningOverrides())
        {
            Option<IContainer> modifiedContainer = None;
            List<int> itemIndiciesToRemove = [];

            if (container.Items is null) continue;
            if (CanContainerBeSkipped(container)) continue;

            for (int i = 0; i < container.Items.Count; ++i)
            {
                if (CanContainerItemBeSkipped(container.Items[i].Item)) continue;

                modifiedContainer.IfNone(() => modifiedContainer = State.PatchMod.Containers.GetOrAddAsOverride(container));
                itemIndiciesToRemove.Add(i);
                ++Stats.ContainerRemover.SpellTomesRemoved;
            }

            modifiedContainer.Match(
                None: NoOp,
                Some: HandleRemoveItemsFromContainer(itemIndiciesToRemove)
            );
        }

        Logger.LogContainerRemoverStats(in Stats.ContainerRemover);
    }

    private void RemoveSpellTomesFromLeveledLists()
    {
        Console.WriteLine("Removing spell tomes from leveled lists...");

        foreach (var leveledItemList in State.LoadOrder.PriorityOrder.LeveledItem().WinningOverrides())
        {
            Option<ILeveledItem> modifiedLeveledItem = None;
            List<int> itemIndiciesToRemove = [];

            if (CanLeveledItemBeSkipped(leveledItemList)) continue;
            if (leveledItemList.Entries is null         ) continue;

            for (int i = 0; i < leveledItemList.Entries.Count; ++i)
            {
                if (CanLeveledItemEntryBeSkipped(leveledItemList.Entries[i])) continue;

                // Spell tome. Modify the leveled item if not already.
                modifiedLeveledItem.IfNone(() => modifiedLeveledItem = State.PatchMod.LeveledItems.GetOrAddAsOverride(leveledItemList));
                itemIndiciesToRemove.Add(i);
                ++Stats.LeveledItemRemover.SpellTomesRemoved;
            }

            modifiedLeveledItem.Match(
                None: NoOp,
                Some: HandleRemoveItemsFromLeveledList(itemIndiciesToRemove)
            );
        }

        Logger.LogLeveledItemRemoverStats(in Stats.LeveledItemRemover);
    }

    public void Run()
    {
        RemovePlacedSpellTomes();
        RemoveSpellTomesFromContainers();
        RemoveSpellTomesFromLeveledLists();
    }
}