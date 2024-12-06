using NUnit.Framework;
using System.Linq;
using WarAndPeace.Core.Models;
using WarAndPeace.Core.Processing;

namespace WarAndPeace.Tests
{
    [TestFixture]
    public class WordTokenizerTests
    {
        private WordTokenizer tokenizer;

        [SetUp]
        public void Setup()
        {
            tokenizer = new WordTokenizer();
        }

        [Test]
        [TestCase("hello world", new[] { "hello", "world" })]
        [TestCase("Hello, World!", new[] { "hello", "world" })]
        [TestCase("hello    world", new[] { "hello", "world" })]
        public void TokenizeText_BasicCases_ReturnsExpectedTokens(string input, string[] expected)
        {
            // Act
            var result = tokenizer.TokenizeText(input).ToArray();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        public void TokenizeText_EmptyOrWhitespace_ReturnsEmptyCollection(string input)
        {
            // Act
            var result = tokenizer.TokenizeText(input);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void TokenizeText_NullInput_ReturnsEmptyCollection()
        {
            // Act
            var result = tokenizer.TokenizeText(null);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void TokenizeText_MultilineInput_TokenizesCorrectly()
        {
            // Arrange
            var input = @"Hello
                         World!
                         How are you?";
            var expected = new[] { "hello", "world", "how", "are", "you" };

            // Act
            var result = tokenizer.TokenizeText(input).ToArray();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TokenizeChunk_ValidChunk_ReturnsTokens()
        {
            // Arrange
            var chunk = new TextChunk(0, "Hello, World!");
            var expected = new[] { "hello", "world" };

            // Act
            var result = tokenizer.TokenizeChunk(chunk).ToArray();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}