namespace WarAndPeace;

public sealed record WordChain(LinkedList<string> Words, int Count);

public static class WordChainOperations
{
    public static WordChain Empty() => new (new LinkedList<string>(), 0);

    public static WordChain Add(WordChain chain, string word)
    {
        var newWords = new LinkedList<string>(chain.Words);
        newWords.AddLast(word);

        return new WordChain(newWords, chain.Count + 1);
    }

    public static bool SaveWords(WordChain chain, string filePath)
    {
        try
        {
            File.WriteAllLines(filePath, chain.Words);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}