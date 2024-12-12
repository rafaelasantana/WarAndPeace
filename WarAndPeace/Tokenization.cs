using System.Text.RegularExpressions;

namespace WarAndPeace;

public static class Tokenization
{
    private static readonly Regex CleanupRegex = new(@"[^\w\s]|\d|_", RegexOptions.Compiled);

    private static readonly Func<string, string> RemovePunctuation = text => CleanupRegex.Replace(text, " ");

    private static readonly Func<string, string> ToLower = text => text.ToLowerInvariant();

    private static readonly Func<string, bool> IsValidWord = word => !string.IsNullOrWhiteSpace(word) && word.Length > 1 && word.All(char.IsLetter);

    private static readonly Func<string, IEnumerable<string>> SplitIntoWords = text => text.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries);
    private static IEnumerable<string> TokenizeText(string text) => SplitIntoWords(RemovePunctuation(text)).Select(ToLower).Where(IsValidWord);
    public static IEnumerable<string> Tokenize(IEnumerable<string> lines) => lines.AsParallel().SelectMany(TokenizeText).Distinct();
}