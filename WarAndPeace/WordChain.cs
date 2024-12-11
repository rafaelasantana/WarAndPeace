namespace WarAndPeace;

public sealed record WordChain(LinkedList<string> Words, int Count);

public static class WordChainOperations
{
    public static WordChain Empty() => new (new LinkedList<string>(), 0);

    public static WordChain Add(WordChain chain, string word)
    {
        chain.Words.AddLast(word);

        return chain with { Count = chain.Count + 1 };
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
    
    public static List<string> ToList(WordChain chain) =>
        [..chain.Words];
}