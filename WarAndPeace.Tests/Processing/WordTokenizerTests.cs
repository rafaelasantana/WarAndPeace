using WarAndPeace.Core.Models;
using WarAndPeace.Core.Processing;

namespace WarAndPeace.Tests.Processing
{
    [TestFixture]
    public class WordTokenizerTests
    {
        private static readonly Func<string[], string[], bool> SequenceEqual = 
            (arr1, arr2) => arr1.SequenceEqual(arr2);

        private static readonly Func<string, string[]> Tokenize = 
            input => WordTokenizer.Tokenize(input).ToArray();

        private static readonly Func<TextChunk?, string[]> TokenizeChunk = 
            chunk => chunk is null 
                ? Array.Empty<string>() 
                : Tokenize(chunk.Content);
            
        private static readonly Func<string, TextChunk> CreateChunk = 
            content => new TextChunk(0, content);

        [Test]
        [TestCase("hello world", new[] { "hello", "world" })]
        [TestCase("Hello, World!", new[] { "hello", "world" })]
        [TestCase("hello    world", new[] { "hello", "world" })]
        public void Tokenize_BasicCases_ReturnsExpectedTokens(string input, string[] expected)
        {
            // Act
            var result = Tokenize(input);
            
            // Assert
            Assert.That(SequenceEqual(result, expected), Is.True);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        public void Tokenize_EmptyOrWhitespace_ReturnsEmptyCollection(string input)
        {
            // Act
            var result = Tokenize(input);
            
            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Tokenize_NullInput_ReturnsEmptyCollection()
        {
            // Act
            var result = Tokenize(null!);
            
            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Tokenize_MultilineInput_TokenizesCorrectly()
        {
            // Arrange
            var input = @"Hello
                         World!
                         How are you?";
            var expected = new[] { "hello", "world", "how", "are", "you" };
            
            // Act
            var result = Tokenize(input);
            
            // Assert
            Assert.That(SequenceEqual(result, expected), Is.True);
        }

        [Test]
        public void Tokenize_ValidChunk_ReturnsTokens()
        {
            // Arrange
            var chunk = CreateChunk("Hello, World!");
            var expected = new[] { "hello", "world" };
            
            // Act
            var result = TokenizeChunk(chunk);
            
            // Assert
            Assert.That(SequenceEqual(result, expected), Is.True);
        }

        [Test]
        public void Tokenize_NullChunk_ReturnsEmptyCollection()
        {
            // Act
            TextChunk? nullChunk = null;
            var result = TokenizeChunk(nullChunk);
            
            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}