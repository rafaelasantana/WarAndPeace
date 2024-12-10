namespace WarAndPeace;

public static class WordCounting
{
    public static WordChain CountWords(IEnumerable<string> lines)
    {
        var wordTree = Tokenization.Tokenize(lines).Aggregate(WordTreeOperations.Empty(), WordTreeOperations.Insert);
        return WordTreeOperations.ToWordChain(wordTree);
    }
}