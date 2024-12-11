namespace WarAndPeace.Tests;

public static class TreeGenerator
{
    public static WordTree GenerateTree(int nodeCount, int seed = 12345)
    {
        var lines = TextGenerator.GenerateLines(nodeCount / 2, seed);
        
        var words = Tokenization.Tokenize(lines)
            .Distinct()
            .Take(nodeCount);
            
        return words.Aggregate(
            WordTreeOperations.Empty(), 
            WordTreeOperations.Insert);
    }
}