namespace SpellTomeRemoverNG;

/// <summary>
/// A logger that suppresses all output.
/// </summary>
public class QuietLogger : IPatcherLogger
{
    public void LogContainerRemoverStats(in ContainerRemoverStats Stats)
    {
        // No output in quiet mode
    }

    public void LogLeveledItemRemoverStats(in LeveledItemRemoverStats Stats)
    {
        // No output in quiet mode
    }

    public void LogMessage(string message)
    {
        // No output in quiet mode
    }

    public void LogPlacedObjectRemoverStats(in PlacedObjectRemoverStats Stats)
    {
        // No output in quiet mode
    }
}