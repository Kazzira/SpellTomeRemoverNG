namespace SpellTomeRemoverNG;

public class ConsolePatcherLogger : IPatcherLogger
{
    public void LogContainerRemoverStats(in ContainerRemoverStats Stats)
    {
        Console.WriteLine($"Skipped {Stats.ContainersSkipped} containers.");
        Console.WriteLine($"Skipped {Stats.SpellTomesSkipped} spell tomes.");
        Console.WriteLine($"Removed {Stats.SpellTomesRemoved} spell tomes from {Stats.ContainersModified} containers.");
        Console.WriteLine("");
    }

    public void LogLeveledItemRemoverStats(in LeveledItemRemoverStats Stats)
    {
        Console.WriteLine($"Skipped {Stats.LeveledListsSkipped} leveled lists.");
        Console.WriteLine($"Skipped {Stats.SpellTomesSkipped} spell tomes.");
        Console.WriteLine($"Removed {Stats.SpellTomesRemoved} spell tomes from {Stats.LeveledListsModified} leveled lists.");
        Console.WriteLine("");
    }

    public void LogPlacedObjectRemoverStats(in PlacedObjectRemoverStats Stats)
    {
        Console.WriteLine($"Skipped {Stats.SpellTomesSkipped} spell tomes.");
        Console.WriteLine($"Removed {Stats.SpellTomesRemoved} placed spell tomes.");
        Console.WriteLine("");
    }
}