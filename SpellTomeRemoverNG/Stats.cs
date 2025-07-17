namespace SpellTomeRemoverNG;

/// <summary>
/// Represents statistics for the leveld item remover process.
/// </summary>
public struct LeveledItemRemoverStats
{
    public LeveledItemRemoverStats() { }

    /// <summary>
    /// The number of leveled lists modified.
    /// </summary>
    public int LeveledListsModified { get; set; } = 0;
    /// <summary>
    /// The number of leveled lists skipped.
    /// </summary>
    public int LeveledListsSkipped { get; set; } = 0;
    /// <summary>
    /// The number of spell tomes skipped.
    /// </summary>
    public int SpellTomesSkipped { get; set; } = 0;
    /// <summary>
    /// The number of spell tomes removed from leveled lists.
    /// </summary>
    public int SpellTomesRemoved { get; set; } = 0;
}

/// <summary>
/// Represents statistics for the container remover process.
/// </summary>
public struct ContainerRemoverStats
{
    public ContainerRemoverStats() { }

    /// <summary>
    /// The number of containers modified.
    /// </summary>
    public int ContainersModified { get; set; } = 0;
    /// <summary>
    /// The number of containers skipped.
    /// </summary>
    public int ContainersSkipped { get; set; } = 0;
    /// <summary>
    /// The number of spell tomes skipped.
    /// </summary>
    public int SpellTomesSkipped { get; set; } = 0;
    /// <summary>
    /// The number of spell tomes removed from containers.
    /// </summary>
    public int SpellTomesRemoved { get; set; } = 0;
}


/// <summary>
/// Represents statistics for the placed object remover process.
/// </summary>
public struct PlacedObjectRemoverStats
{
    public PlacedObjectRemoverStats() { }

    /// <summary>
    /// The number of spell tomes removed.
    /// </summary>
    public int SpellTomesRemoved { get; set; } = 0;
    /// <summary>
    /// The number of spell tomes skipped.
    /// </summary>
    public int SpellTomesSkipped { get; set; } = 0;
}


/// <summary>
/// Represents the statistics collected during the patching process.
/// <summary>
public struct PatcherStats
{
    public PatcherStats() { }

    /// <summary>
    /// Statistics for the leveled item remover.
    /// </summary>
    public LeveledItemRemoverStats LeveledItemRemover = new();
    /// <summary>
    /// Statistics for the container remover.
    /// </summary>
    public ContainerRemoverStats ContainerRemover = new();
    /// <summary>
    /// Statistics for the placed object remover.
    /// </summary>
    public PlacedObjectRemoverStats PlacedObjectRemover = new();
}