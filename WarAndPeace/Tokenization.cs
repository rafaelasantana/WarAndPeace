using System.Text.RegularExpressions;

namespace WarAndPeace;

public static class Tokenization
{
    public static readonly Func<string, string> RemovePunctuation = text =>
    {
        var cleanupRegex = new Regex(@"[^\w\s]|\d|_", RegexOptions.Compiled);
        return cleanupRegex.Replace(text, "");
    };

    public static readonly Func<string, string> ToLower = text => text.ToLowerInvariant();

    public static readonly Func<string, bool> IsValidWord = word => !string.IsNullOrWhiteSpace(word) && word.Length > 1 && word.All(char.IsLetter);

    public static readonly Func<string, IEnumerable<string>> SplitIntoWords = text => text.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries);

    public static IEnumerable<string> TokenizeText(string text) => SplitIntoWords(text).Select(RemovePunctuation).Select(ToLower).Where(IsValidWord);

    public static IEnumerable<string> Tokenize(IEnumerable<string> lines) => lines.AsParallel().SelectMany(TokenizeText);
}