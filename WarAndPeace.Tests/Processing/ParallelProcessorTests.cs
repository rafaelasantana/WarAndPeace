using WarAndPeace.Core.Models;
using WarAndPeace.Core.Processing;

namespace WarAndPeace.Tests.Processing
{
    [TestFixture]
    public class ParallelProcessorTests
    {
        // Test data factories
        private static readonly Func<TextChunk[]> CreateBasicChunks = () => new[]
        {
            new TextChunk(0, "Hello world"),
            new TextChunk(12, "world hello there"),
            new TextChunk(24, "there hello")
        };

        private static readonly Func<TextChunk[]> CreateDuplicateChunks = () => new[]
        {
            new TextChunk(0, "hello hello hello"),
            new TextChunk(12, "hello world world"),
            new TextChunk(24, "world hello")
        };

        private static readonly Func<TextChunk[]> CreateOrderedChunks = () => new[]
        {
            new TextChunk(0, "zebra alpha charlie"),
            new TextChunk(12, "beta delta")
        };

        // Test helpers
        private static readonly Func<ProcessingResult, bool> ValidateProcessingTime =
            result => result.ProcessingTimeMs >= 0;

        private static readonly Func<IEnumerable<string>, IEnumerable<string>, bool> AreEquivalent =
            (actual, expected) => actual.OrderBy(x => x).SequenceEqual(expected.OrderBy(x => x));

        [Test]
        public async Task ProcessChunksAsync_MultipleChunks_ProcessesCorrectly()
        {
            // Arrange
            var chunks = CreateBasicChunks();
            var expectedWords = new[] { "hello", "world", "there" };

            // Act
            var result = await ParallelProcessor.ProcessChunksAsync(chunks);

            // Assert
            Assert.Multiple(() =>
            {
                if (result.UniqueWords != null)
                {
                    Assert.That(result.UniqueWords, Has.Count.EqualTo(3));
                    Assert.That(AreEquivalent(result.UniqueWords, expectedWords), Is.True);
                }

                Assert.That(ValidateProcessingTime(result), Is.True);
            });
        }

        [Test]
        public async Task ProcessChunksAsync_EmptyInput_ReturnsEmptyResult()
        {
            // Arrange
            var emptyChunks = Array.Empty<TextChunk>();

            // Act
            var result = await ParallelProcessor.ProcessChunksAsync(emptyChunks);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.UniqueWords, Is.Empty);
                Assert.That(result.TotalUniqueWords, Is.EqualTo(0));
                Assert.That(result.ProcessingTimeMs, Is.GreaterThanOrEqualTo(0));
                Assert.That(result.ProcessingTimeMs, Is.LessThan(100)); // Reasonable upper bound for empty processing
            });
        }

        [Test]
        public async Task ProcessChunksAsync_DuplicateWords_ReturnsUniqueWords()
        {
            // Arrange
            var chunks = CreateDuplicateChunks();
            var expectedWords = new[] { "hello", "world" };

            // Act
            var result = await ParallelProcessor.ProcessChunksAsync(chunks);

            // Assert
            Assert.Multiple(() =>
            {
                if (result.UniqueWords == null) return;
                Assert.That(result.UniqueWords, Has.Count.EqualTo(2));
                Assert.That(AreEquivalent(result.UniqueWords, expectedWords), Is.True);
            });
        }

        [Test]
        public async Task ProcessChunksAsync_OrderedOutput_MaintainsAlphabeticalOrder()
        {
            // Arrange
            var chunks = CreateOrderedChunks();
            var expectedWords = new[] { "alpha", "beta", "charlie", "delta", "zebra" };

            // Act
            var result = await ParallelProcessor.ProcessChunksAsync(chunks);

            // Assert
            Assert.That(result.UniqueWords, Is.EqualTo(expectedWords));
        }

        [Test]
        public async Task ProcessChunksAsync_NullInput_ReturnsEmptyResult()
        {
            // Act
            var result = await ParallelProcessor.ProcessChunksAsync(null);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.UniqueWords, Is.Empty);
                Assert.That(result.TotalUniqueWords, Is.EqualTo(0));
                Assert.That(result.ProcessingTimeMs, Is.GreaterThanOrEqualTo(0));
                Assert.That(result.ProcessingTimeMs, Is.LessThan(100));
            });
        }
    }
}