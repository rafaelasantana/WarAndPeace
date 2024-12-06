namespace WarAndPeace.Core.Models;

public class ProcessingResult
{
    public List<string> UniqueWords { get; init; }
    public double ProcessingTimeMs { get; init; }
    public int TotalUniqueWords { get; init; }
}