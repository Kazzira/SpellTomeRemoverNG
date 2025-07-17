namespace SpellTomeRemoverNG;

public interface IPatcherLogger
{
    public void LogPlacedObjectRemoverStats(in PlacedObjectRemoverStats Stats);
    public void LogLeveledItemRemoverStats(in LeveledItemRemoverStats Stats);
    public void LogContainerRemoverStats(in ContainerRemoverStats Stats);
}