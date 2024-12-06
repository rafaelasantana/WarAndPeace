using System.Collections.Immutable;

namespace WarAndPeace.Core.Models;

public class ProcessingResult
{
    public ImmutableList<string>? UniqueWords { get; init; }
    public double ProcessingTimeMs { get; init; }
    public int TotalUniqueWords { get; init; }
}