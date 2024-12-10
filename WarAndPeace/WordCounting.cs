namespace WarAndPeace;

public static class WordCounting
{
    public static WordChain CountWords(IEnumerable<string> lines)
    {
        var tokens = Tokenization.Tokenize(lines);
        var wordTree = tokens.Aggregate(WordTreeOperations.Empty(), WordTreeOperations.Insert);
        return WordTreeOperations.ToWordChain(wordTree);
    }
}