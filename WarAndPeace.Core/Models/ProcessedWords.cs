using System.Collections.Immutable;

namespace WarAndPeace.Core.Models;

public record ProcessedWords(ImmutableHashSet<string> Words);

