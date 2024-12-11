namespace WarAndPeace;

public static class Tokenization
{
    private const int ChunkSize = 1000;
    private static readonly char[] WordSeparators = [' ', '\n', '\r', '\t'];
    
    private static readonly Func<string, string> RemovePunctuation = text => new string(text.Where(c => char.IsLetter(c) || char.IsWhiteSpace(c)).ToArray());
    
    private static readonly Func<string, bool> IsValidWord = word => !string.IsNullOrWhiteSpace(word) && word.Length > 1 && word.All(char.IsLetter);
    
    private static readonly Func<string, IEnumerable<string>> SplitIntoWords = text => text.Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries);
    
    private static readonly Func<string, string> ToLowerCase = text => text.ToLowerInvariant();
    
    private static readonly Func<string, IEnumerable<string>> TokenizeText = text => SplitIntoWords(RemovePunctuation(text)).Select(ToLowerCase).Where(IsValidWord);
    
    public static readonly Func<IEnumerable<string>, IEnumerable<string>> Tokenize = lines =>
        lines.Chunk(ChunkSize)
            .AsParallel()
            .SelectMany(chunk => chunk
                .SelectMany(TokenizeText)
                .ToList())
            .ToList();
}