using static System.String;

using static WarAndPeace.NodeColor;

namespace WarAndPeace;

public enum NodeColor
{
    Red,
    Black
}

public abstract record WordTree;

public sealed record EmptyNode : WordTree;

public sealed record Node(string Word, NodeColor Color, WordTree Left, WordTree Right) : WordTree;

public static class WordTreeOperations
{
    public static WordTree Empty() => new EmptyNode();

    public static WordTree Insert(WordTree tree, string word)
    {
        return MakeBlack(InsertRec(tree, word));

        static WordTree Balance(NodeColor color, WordTree left, string word, WordTree right)
        {
            return (color, left, right) switch
            {
                // Left-Left Case
                (Black, Node({ } lv, Red, Node({ } llv, Red, var lll, var llr), var lr), _) => new Node(lv, Red, new Node(llv, Black, lll, llr), new Node(word, Black, lr, right)),

                // Left-Right Case
                (Black, Node({ } lv, Red, var ll, Node({ } lrv, Red, var lrl, var lrr)), _) => new Node(lrv, Red, new Node(lv, Black, ll, lrl), new Node(word, Black, lrr, right)),

                // Right-Left Case
                (Black, _, Node({ } rv, Red, Node({ } rlv, Red, var rll, var rlr), var rr)) => new Node(rlv, Red, new Node(word, Black, left, rll), new Node(rv, Black, rlr, rr)),

                // Right-Right Case
                (Black, _, Node({ } rv, Red, var rl, Node({ } rrv, Red, var rrl, var rrr))) => new Node(rv, Red, new Node(word, Black, left, rl), new Node(rrv, Black, rrl, rrr)),

                // Default: No balancing needed
                _ => new Node(word, color, left, right)
            };
        }

        static WordTree InsertRec(WordTree tree, string word)
        {
            return tree switch
            {
                EmptyNode => new Node(word, Red, new EmptyNode(), new EmptyNode()),
                Node({ } nodeWord, var color, var left, var right) => Compare(word, nodeWord, StringComparison.Ordinal) switch
                {
                    < 0 => Balance(color, InsertRec(left, word), nodeWord, right),
                    > 0 => Balance(color, left, nodeWord, InsertRec(right, word)),
                    _ => tree
                },

                _ => throw new InvalidOperationException("Unexpected tree structure")
            };
        }

        static WordTree MakeBlack(WordTree tree) =>
            tree switch
            {
                Node({ } word, _, var left, var right) => new Node(word, Black, left, right),
                EmptyNode => tree,
                _ => throw new InvalidOperationException("Unexpected tree structure")
            };
    }

    public static WordChain ToWordChain(WordTree tree)
    {
        return Traverse(tree, WordChainOperations.Empty());

        static WordChain InOrder(string word, WordTree left, WordTree right, WordChain wordChain)
        {
            var leftChain = Traverse(left, wordChain);
            var currentChain = WordChainOperations.Add(leftChain, word);

            return Traverse(right, currentChain);
        }

        static WordChain Traverse(WordTree node, WordChain chain)
        {
            return node switch
            {
                EmptyNode => chain,

                // Recursive case: Node
                Node(var word, _, var left, var right) => InOrder(word, left, right, chain),

                _ => throw new InvalidOperationException("Unexpected tree structure")
            };
        }
    }
}