namespace SpellTomeRemoverNG;

/// <summary>
/// Console logger for the patcher.
/// </summary>
public class ConsolePatcherLogger : IPatcherLogger
{
    public void LogMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void LogContainerRemoverStats(in ContainerRemoverStats Stats)
    {
        LogMessage($"Skipped {Stats.ContainersSkipped} containers.");
        LogMessage($"Skipped {Stats.SpellTomesSkipped} spell tomes.");
        LogMessage($"Removed {Stats.SpellTomesRemoved} spell tomes from {Stats.ContainersModified} containers.");
        LogMessage("");
    }

    public void LogLeveledItemRemoverStats(in LeveledItemRemoverStats Stats)
    {
        LogMessage($"Skipped {Stats.LeveledListsSkipped} leveled lists.");
        LogMessage($"Skipped {Stats.SpellTomesSkipped} spell tomes.");
        LogMessage($"Removed {Stats.SpellTomesRemoved} spell tomes from {Stats.LeveledListsModified} leveled lists.");
        LogMessage("");
    }

    public void LogPlacedObjectRemoverStats(in PlacedObjectRemoverStats Stats)
    {
        LogMessage($"Skipped {Stats.SpellTomesSkipped} spell tomes.");
        LogMessage($"Removed {Stats.SpellTomesRemoved} placed spell tomes.");
        LogMessage("");
    }
}