namespace WarAndPeace.Tests;
[TestFixture]
public class TokenizationTests
{
    [Test]
    public void Tokenize_PropertyBasedTests()
    {
        var lines = TextGenerator.GenerateLines(10, seed: 12345).ToList();
        var result = Tokenization.Tokenize(lines);

        Assert.Multiple(() =>
        {
            var enumerable = result as string[] ?? result.ToArray();
            Assert.That(enumerable.All(word => word == word.ToLower()), 
                Is.True, "All words should be lowercase");
            
            Assert.That(enumerable.All(word => word.All(char.IsLetter)), 
                Is.True, "Words should only contain letters");
            
            Assert.That(enumerable.All(word => word.Length >= 2), 
                Is.True, "All words should be at least two characters long");

            Assert.That(enumerable, Is.Not.Empty, "Should produce at least some words");
        });
    }

    [Test]
    public async Task Tokenize_BasicInput()
    {
        var lines = new List<string>
        {
            "The QUICK brown FOX jumps over the LAZY dog!",
            "The quick BROWN fox JUMPS over THE lazy DOG.",
            "Some-punctuation, and... multiple    spaces",
        };

        var result = Tokenization.Tokenize(lines);

        await Verify(result);
    }

    [Test]
    public async Task Tokenize_GeneratedComplexInput()
    {
        var lines = TextGenerator.GenerateLines(5, seed: 12345).ToList();
        
        var result = Tokenization.Tokenize(lines);

        var enumerable = result as string[] ?? result.ToArray();
        await Verify(new
        {
            InputLineCount = lines.Count,
            InputLines = lines,
            TokenizedWords = enumerable,
            AllWordsAreLowercase = enumerable.All(word => word == word.ToLower()),
            AllWordsOnlyLetters = enumerable.All(word => word.All(char.IsLetter)),
            AllWordsValidLength = enumerable.All(word => word.Length >= 2)
        });
    }

    [Test]
    public async Task Tokenize_EmptyInput()
    {
        var result = Tokenization.Tokenize([]);
        await Verify(result);
    }

    [Test]
    public async Task Tokenize_WithExcessivePunctuation()
    {
        var lines = new List<string>
        {
            "!!!Hello!!!World!!!",
            "...What...Is...This...",
            "***Special***Case***",
            "Multiple!!!Punctuation!!!Marks",
            "In!!!Between!!!Words"
        };

        var result = Tokenization.Tokenize(lines);
        await Verify(result);
    }

    [Test]
    public async Task Tokenize_ParallelProcessing()
    {
        var lines = TextGenerator.GenerateLines(1000, seed: 54321).ToList();
        
        var result = Tokenization.Tokenize(lines).OrderBy(x => x).ToList();

        await Verify(new
        {
            ProcessedWordCount = result.Count,
            SampleWords = result.Take(10),
            AllWordsAreLowercase = result.All(word => word == word.ToLower()),
            AllWordsOnlyLetters = result.All(word => word.All(char.IsLetter)),
            AllWordsValidLength = result.All(word => word.Length >= 2)
        });
    }
}