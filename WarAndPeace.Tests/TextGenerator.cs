namespace WarAndPeace.Tests;

public static class TextGenerator 
{
    private static Random CreateSeededRandom(int seed = 12345) => new Random(seed);
    
    private static string GenerateWord(int length, int seed = 12345)
    {
        var random = CreateSeededRandom(seed);
        return new string(Enumerable.Range(0, length)
            .Select(_ => (char)('a' + random.Next(26)))
            .ToArray());
    }
            
    private static string GenerateTextWithPunctuation(int seed = 12345)
    {
        var random = CreateSeededRandom(seed);
        var punctuationMarks = new[] { ".", ",", "!", "?", ";", ":", "-", "'", "\"" };
        var words = Enumerable.Range(0, random.Next(5, 15))
            .Select(i => GenerateWord(random.Next(1, 10), seed + i))
            .Select(w => random.Next(2) == 0 ? w.ToUpper() : w);
            
        return string.Join(" ", words
            .Select(w => w + (random.Next(3) == 0 ? 
                punctuationMarks[random.Next(punctuationMarks.Length)] : "")));
    }
    
    public static IEnumerable<string> GenerateLines(int lineCount, int seed = 12345) =>
        Enumerable.Range(0, lineCount)
            .Select(i => GenerateTextWithPunctuation(seed + i));
}