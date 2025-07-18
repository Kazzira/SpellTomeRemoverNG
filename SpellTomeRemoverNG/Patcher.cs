using LanguageExt;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using static LanguageExt.Prelude;

namespace SpellTomeRemoverNG;

/// <summary>
/// Patcher class for removing spell tomes from Skyrim mods.
/// </summary>
/// <param name="PatcherState">The state of the patcher.</param>
/// <param name="PatcherLogger">The logger for the patcher.</param>
/// <remarks>
/// This class implements the patching logic for removing spell tomes from containers, leveled lists, and placed objects in Skyrim mods.
/// It uses the Mutagen framework to interact with Skyrim mod data.
/// </remarks>
public class Patcher(
    IPatcherState<ISkyrimMod, ISkyrimModGetter> PatcherState,
    IPatcherLogger PatcherLogger
)
{
    /// <summary>
    /// The state of the patcher.
    /// </summary>
    private readonly IPatcherState<ISkyrimMod, ISkyrimModGetter> State = PatcherState;
    /// <summary>
    /// The logger for the patcher.
    /// </summary>
    private readonly IPatcherLogger Logger = PatcherLogger;

    /// <summary>
    /// Statistics for the patching process.
    /// </summary>
    private PatcherStats Stats = new();

    /// <summary>
    /// Returns true if the container can be skipped.
    /// <param name="container">The container to check.</param>
    /// <returns>True if the container can be skipped, false otherwise.</returns>
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

    /// <summary>
    /// Returns true if the container item can be skipped.
    /// </summary>
    /// <param name="containerItem">The container item to check.</param>
    /// <returns>True if the container item can be skipped, false otherwise.</returns>
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

    /// <summary>
    /// Returns true if the leveled item can be skipped.
    /// </summary>
    /// <param name="leveledItem">The leveled item to check.</param>
    /// <returns>True if the leveled item can be skipped, false otherwise.</returns
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

    /// <summary>
    /// Returns true if the leveled item entry can be skipped.
    /// </summary>
    /// <param name="entry">The leveled item entry to check.</param>
    /// <returns>True if the leveled item entry can be skipped, false otherwise.</returns>
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

    /// <summary>
    /// Returns true if the placed spell tome can be skipped.
    /// </summary>
    /// <param name="placedObject">The placed object to check.</param>
    /// <returns>True if the placed spell tome can be skipped, false otherwise.</returns
    private bool CanPlacedSpellTomeBeSkipped(IPlacedObjectGetter placedObject)
    {
        // Item already disabled?
        if (placedObject.MajorRecordFlagsRaw == 0x0000_0800) return true;
        // A book?
        if (!placedObject.Base.TryResolve<IBookGetter>(State.LinkCache, out var placedBook)) return true;
        // A spell tome?
        if (placedBook.Teaches is not IBookSpellGetter) return true;

        // Placed item or book is from a blacklisted mod?
        if (IsPlacedItemBlacklisted(placedObject) || IsBookBlacklisted(placedBook))
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
        Func<PluginBlacklistEntry, bool> predicate
    )
    {
        return Settings.Instance.Value.PluginBlackList
                                      .Where(entry => entry.PluginName == formKeyGetter.FormKey.ModKey.FileName)
                                      .ToOption()
                                      .Map(predicate)
                                      .IfNone(false);
    }

    /// <summary>
    /// Returns true if the container is blacklisted.
    /// </summary>
    /// <param name="container">The container to check.</param>
    /// <returns>True if the container is blacklisted, false otherwise.</returns>
    private static bool IsContainerBlacklisted(IContainerGetter container) =>
        IsObjectBlacklisted(container, entry => entry.SkipContainers);

    /// <summary>
    /// Returns true if the leveled list is blacklisted.
    /// </summary>
    /// <param name="leveledItem">The leveled item to check.</param>
    /// <returns>True if the leveled list is blacklisted, false otherwise.</returns>
    private static bool IsLeveledListBlacklisted(ILeveledItemGetter leveledItem) =>
        IsObjectBlacklisted(leveledItem, entry => entry.SkipLeveledLists);

    /// <summary>
    /// Returns true if the placed item is blacklisted.
    /// </summary>
    /// <param name="placedObject">The placed object to check.</param>
    /// <returns>True if the placed item is blacklisted, false otherwise.</returns>
    private static bool IsPlacedItemBlacklisted(IPlacedObjectGetter placedObject) =>
        IsObjectBlacklisted(placedObject, entry => entry.SkipPlacedObjects);

    /// <summary>
    /// Returns true if the book is blacklisted.
    /// </summary>
    /// <param name="book">The book to check.</param>
    /// <returns>True if the book is blacklisted, false otherwise.</returns>
    private static bool IsBookBlacklisted(IBookGetter book) =>
        IsObjectBlacklisted(book, entry => entry.SkipBooks);

    /// <summary>
    /// Handles the removal of items from a container.
    /// </summary>
    /// <param name="itemIndicesToRemove">The indices of the items to remove.</param>
    /// <returns>An action that removes items from the container.</returns>
    private Action<IContainer> HandleRemoveItemsFromContainer(List<int> itemIndicesToRemove)
    {
        return ctnr =>
        {
            ++Stats.ContainerRemover.ContainersModified;
            RemoveItemsFromContainer(ctnr, itemIndicesToRemove);
        };
    }

    /// <summary>
    /// Handles the removal of items from a leveled list.
    /// </summary>
    /// <param name="itemIndicesToRemove">The indices of the items to remove.</param>
    /// <returns>An action that removes items from the leveled list.</returns>
    private Action<ILeveledItem> HandleRemoveItemsFromLeveledList(List<int> itemIndicesToRemove)
    {
        return lvli =>
        {
            ++Stats.LeveledItemRemover.LeveledListsModified;
            RemoveItemsFromLeveledList(lvli, itemIndicesToRemove);
        };
    }

    /// <summary>
    /// No operation method.
    /// </summary>
    private static void NoOp() { }

    /// <summary>
    /// Removes items from a container.
    /// </summary>
    /// <param name="container">The container to modify.</param>
    /// <param name="itemIndicesToRemove">The indices of the items to remove.</param>
    private static void RemoveItemsFromContainer(IContainer container, List<int> itemIndicesToRemove)
    {
        if (container.Items is null) return;

        foreach (var index in itemIndicesToRemove.OrderByDescending(x => x))
        {
            container.Items.RemoveAt(index);
        }
    }

    /// <summary>
    /// Removes items from a leveled list.
    /// </summary>
    /// <param name="leveledItem">The leveled item to modify.</param>
    /// <param name="itemIndicesToRemove">The indices of the items to remove.</param>
    private static void RemoveItemsFromLeveledList(ILeveledItem leveledItem, List<int> itemIndicesToRemove)
    {
        if (leveledItem.Entries is null) return;

        foreach (var index in itemIndicesToRemove.OrderByDescending(x => x))
        {
            leveledItem.Entries.RemoveAt(index);
        }
    }

    /// <summary>
    /// Removes placed spell tomes from the game.
    /// </summary>
    private void RemovePlacedSpellTomes()
    {
        Logger.LogMessage("Removing placed spell tomes...");

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

    /// <summary>
    /// Removes spell tomes from containers.
    /// </summary>
    private void RemoveSpellTomesFromContainers()
    {
        Logger.LogMessage("Removing spell tomes from containers...");

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

    /// <summary>
    /// Removes spell tomes from leveled lists.
    /// </summary>
    private void RemoveSpellTomesFromLeveledLists()
    {
        Logger.LogMessage("Removing spell tomes from leveled lists...");

        foreach (var leveledItemList in State.LoadOrder.PriorityOrder.LeveledItem().WinningOverrides())
        {
            Option<ILeveledItem> modifiedLeveledItem = None;
            List<int> itemIndiciesToRemove = [];

            if (CanLeveledItemBeSkipped(leveledItemList)) continue;
            if (leveledItemList.Entries is null) continue;

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

    /// <summary>
    /// Runs the patcher to remove spell tomes from Skyrim mods.
    /// </summary>
    public void Run()
    {
        RemovePlacedSpellTomes();
        RemoveSpellTomesFromContainers();
        RemoveSpellTomesFromLeveledLists();
    }
}