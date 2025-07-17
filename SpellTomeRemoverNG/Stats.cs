namespace SpellTomeRemoverNG;

public struct LeveledItemRemoverStats
{
    public LeveledItemRemoverStats() { }
    public int LeveledListsModified { get; set; } = 0;
    public int LeveledListsSkipped { get; set; } = 0;
    public int SpellTomesSkipped { get; set; } = 0;
    public int SpellTomesRemoved { get; set; } = 0;
}

public struct ContainerRemoverStats
{
    public ContainerRemoverStats() { }

    public int ContainersModified { get; set; } = 0;
    public int ContainersSkipped { get; set; } = 0;
    public int SpellTomesSkipped { get; set; } = 0;
    public int SpellTomesRemoved { get; set; } = 0;
}


public struct PlacedObjectRemoverStats
{
    public PlacedObjectRemoverStats() { }

    public int SpellTomesRemoved { get; set; } = 0;
    public int SpellTomesSkipped { get; set; } = 0;
}


public struct PatcherStats
{
    public PatcherStats() { }

    public LeveledItemRemoverStats LeveledItemRemover = new();
    public ContainerRemoverStats ContainerRemover = new();
    public PlacedObjectRemoverStats PlacedObjectRemover = new();
}