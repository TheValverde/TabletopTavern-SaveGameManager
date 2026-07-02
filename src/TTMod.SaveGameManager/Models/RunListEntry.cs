namespace TTMod.SaveGameManager.Models;

internal enum RunStorageKind
{
    Active,
    Parked,
}

internal sealed class RunListEntry
{
    internal RunListEntry(
        string runId,
        RunStorageKind storageKind,
        string heroName,
        int heroId,
        string difficulty,
        int bookNumber,
        int layer,
        int chaptersCompleted,
        int seed,
        int goldAmount,
        string savedUtc,
        int? loopsCompleted = null)
    {
        RunId = runId;
        StorageKind = storageKind;
        HeroName = heroName;
        HeroId = heroId;
        Difficulty = difficulty;
        BookNumber = bookNumber;
        Layer = layer;
        ChaptersCompleted = chaptersCompleted;
        Seed = seed;
        GoldAmount = goldAmount;
        SavedUtc = savedUtc;
        LoopsCompleted = loopsCompleted;
    }

    internal string RunId { get; }
    internal RunStorageKind StorageKind { get; }
    internal string HeroName { get; }
    internal int HeroId { get; }
    internal string Difficulty { get; }
    internal int BookNumber { get; }
    internal int Layer { get; }
    internal int ChaptersCompleted { get; }
    internal int Seed { get; }
    internal int GoldAmount { get; }
    internal string SavedUtc { get; }
    internal int? LoopsCompleted { get; }

    internal bool IsActive => StorageKind == RunStorageKind.Active;
}
