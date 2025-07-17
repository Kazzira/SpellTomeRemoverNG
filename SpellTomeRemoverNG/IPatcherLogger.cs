namespace SpellTomeRemoverNG;

/// <summary>
/// Interface for patcher logging.
/// This interface defines methods for logging messages and statistics related to the patching process.
/// </summary>
public interface IPatcherLogger
{
    /// <summary>
    /// Logs a message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogMessage(string message);

    /// <summary>
    /// Logs statistics for the placed object remover.
    /// </summary>
    /// <param name="Stats">The statistics to log.</param>
    /// <remarks>
    /// This method is used to log the statistics related to placed object removals,
    /// including the number of spell tomes skipped and removed.
    /// </remarks>
    public void LogPlacedObjectRemoverStats(in PlacedObjectRemoverStats Stats);

    /// <summary>
    /// Logs statistics for the leveled item remover.
    /// </summary>
    /// <param name="Stats">The statistics to log.</param>
    /// <remarks>
    /// This method is used to log the statistics related to leveled item removals,
    /// including the number of leveled lists skipped, spell tomes skipped, and spell tomes removed.
    /// </remarks>
    public void LogLeveledItemRemoverStats(in LeveledItemRemoverStats Stats);

    /// <summary>
    /// Logs statistics for the container remover.
    /// </summary>
    /// <param name="Stats">The statistics to log.</param>
    /// <remarks>
    /// This method is used to log the statistics related to container removals,
    /// including the number of containers skipped and removed.
    /// </remarks>
    public void LogContainerRemoverStats(in ContainerRemoverStats Stats);
}