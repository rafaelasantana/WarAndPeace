namespace WarAndPeace.Tests;

[TestFixture]
public class WordTreeTests
{    
    private static bool IsRedNode(WordTree node) => 
        node is Node { Color: NodeColor.Red };

    private static bool IsBlackNode(WordTree node) =>
        node is EmptyNode or Node { Color: NodeColor.Black };

    private static int BlackHeight(WordTree node)
    {
        return node switch
        {
            EmptyNode => 1,
            Node n => (n.Color == NodeColor.Black ? 1 : 0) +
                     Math.Max(BlackHeight(n.Left), BlackHeight(n.Right)),
            _ => throw new ArgumentException("Invalid node type")
        };
    }

    private static bool HasValidBlackHeight(WordTree node)
    {
        return node switch
        {
            EmptyNode => true,
            Node n => BlackHeight(n.Left) == BlackHeight(n.Right) &&
                     HasValidBlackHeight(n.Left) &&
                     HasValidBlackHeight(n.Right),
            _ => throw new ArgumentException("Invalid node type")
        };
    }

    private static bool HasNoConsecutiveRedNodes(WordTree node)
    {
        return node switch
        {
            EmptyNode => true,
            Node n => (!IsRedNode(n) || (IsBlackNode(n.Left) && IsBlackNode(n.Right))) &&
                     HasNoConsecutiveRedNodes(n.Left) &&
                     HasNoConsecutiveRedNodes(n.Right),
            _ => throw new ArgumentException("Invalid node type")
        };
    }

    private static bool IsBinarySearchTree(WordTree node)
    {
        return InRange(node, null, null);

        static bool InRange(WordTree node, string? minValue, string? maxValue)
        {
            return node switch
            {
                EmptyNode => true,
                Node n => (minValue == null || string.CompareOrdinal(n.Word, minValue) > 0) &&
                          (maxValue == null || string.CompareOrdinal(n.Word, maxValue) < 0) &&
                          InRange(n.Left, minValue, n.Word) &&
                          InRange(n.Right, n.Word, maxValue),
                _ => throw new ArgumentException("Invalid node type")
            };
        }
    }

    [Test]
    public void PropertyTest_EmptyTree()
    {
        var tree = WordTreeOperations.Empty();
        Assert.That(tree, Is.InstanceOf<EmptyNode>());
    }

    [Test]
    public void PropertyTest_RootIsBlack()
    {
        var nodeCounts = new[] { 10, 50, 100 };
        foreach (var count in nodeCounts)
        {
            var tree = TreeGenerator.GenerateTree(count);
            Assert.Multiple(() =>
            {
                Assert.That(tree, Is.InstanceOf<Node>());
                Assert.That(((Node)tree).Color, Is.EqualTo(NodeColor.Black),
                    $"Root should be black for tree with {count} nodes");
            });
        }
    }

    [Test]
    public void PropertyTest_NoConsecutiveRedNodes()
    {
        var nodeCounts = new[] { 10, 50, 100 };
        foreach (var count in nodeCounts)
        {
            var tree = TreeGenerator.GenerateTree(count);
            Assert.That(HasNoConsecutiveRedNodes(tree), Is.True,
                $"Tree with {count} nodes has consecutive red nodes");
        }
    }

    [Test]
    public void PropertyTest_BlackHeightIsBalanced()
    {
        var nodeCounts = new[] { 10, 50, 100 };
        foreach (var count in nodeCounts)
        {
            var tree = TreeGenerator.GenerateTree(count);
            Assert.That(HasValidBlackHeight(tree), Is.True,
                $"Tree with {count} nodes has unbalanced black height");
        }
    }

    [Test]
    public void PropertyTest_MaintainsBSTProperty()
    {
        var nodeCounts = new[] { 10, 50, 100 };
        foreach (var count in nodeCounts)
        {
            var tree = TreeGenerator.GenerateTree(count);
            Assert.That(IsBinarySearchTree(tree), Is.True,
                $"Tree with {count} nodes violates BST property");
        }
    }

    [Test]
    public void PropertyTest_InsertPreservesOrder()
    {
        var lines = TextGenerator.GenerateLines(50);
        var words = Tokenization.Tokenize(lines).Distinct().Take(100).ToList();
        
        var tree = WordTreeOperations.Empty();
        var expectedWords = new SortedSet<string>();
        
        foreach (var word in words)
        {
            tree = WordTreeOperations.Insert(tree, word);
            expectedWords.Add(word);
            
            var wordChain = WordTreeOperations.ToWordChain(tree);
            Assert.That(wordChain.Count, Is.EqualTo(expectedWords.Count),
                "Word count mismatch after insertion");
            
            var wordChainList = WordChainOperations.ToList(wordChain);
            var expectedList = expectedWords.ToList();
            
            Assert.That(wordChainList, Is.EqualTo(expectedList),
                "Tree does not maintain correct word order after insertion");
        }
    }

    [Test]
    public void PropertyTest_InsertHandlesDuplicates()
    {
        var lines = TextGenerator.GenerateLines(25);
        var words = Tokenization.Tokenize(lines).Distinct().Take(50).ToList();
        var duplicatedWords = words.Concat(words).ToList();
        
        var tree = duplicatedWords.Aggregate(
            WordTreeOperations.Empty(),
            WordTreeOperations.Insert);
            
        var wordChain = WordTreeOperations.ToWordChain(tree);
        Assert.That(wordChain.Count, Is.EqualTo(words.Distinct().Count()),
            "Tree should not contain duplicates");
    }

    [Test]
    public void PropertyTest_TreeMaintainsSortedOrder()
    {
        var tree = TreeGenerator.GenerateTree(100);
        var wordChain = WordTreeOperations.ToWordChain(tree);
        var wordList = WordChainOperations.ToList(wordChain);
        
        Assert.That(wordList, Is.Ordered,
            "Words in the tree should be maintained in sorted order");
    }
}