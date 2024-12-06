// using WarAndPeace.Core.Models;
// using WarAndPeace.Core.Processing;
//
// namespace WarAndPeace.Tests;
//
//
//     [TestFixture]
//     public class ParallelProcessorTests
//     {
//         private ParallelProcessor processor;
//         private WordTokenizer tokenizer;
//
//         [SetUp]
//         public void Setup()
//         {
//             tokenizer = new WordTokenizer();
//             processor = new ParallelProcessor(tokenizer);
//         }
//
//         [Test]
//         public async Task ProcessWithStatisticsAsync_MultipleChunks_ProcessesCorrectly()
//         {
//             // Arrange
//             var chunks = new[]
//             {
//                 new TextChunk(0, "Hello world"),
//                 new TextChunk(12, "world hello there"),
//                 new TextChunk(24, "there hello")
//             };
//
//             // Act
//             var result = await processor.ProcessWithStatisticsAsync(chunks);
//
//             // Assert
//             Assert.That(result.UniqueWords.Count, Is.EqualTo(3));
//             Assert.That(result.UniqueWords, Does.Contain("hello"));
//             Assert.That(result.UniqueWords, Does.Contain("world"));
//             Assert.That(result.UniqueWords, Does.Contain("there"));
//             Assert.That(result.ProcessingTimeMs, Is.GreaterThanOrEqualTo(0));
//         }
//
//         [Test]
//         public async Task ProcessWithStatisticsAsync_EmptyInput_ReturnsEmptyResult()
//         {
//             // Arrange
//             var chunks = new TextChunk[0];
//
//             // Act
//             var result = await processor.ProcessWithStatisticsAsync(chunks);
//
//             // Assert
//             Assert.That(result.UniqueWords, Is.Empty);
//             Assert.That(result.TotalUniqueWords, Is.EqualTo(0));
//         }
//
//         [Test]
//         public async Task ProcessWithStatisticsAsync_DuplicateWords_ReturnsUniqueWords()
//         {
//             // Arrange
//             var chunks = new[]
//             {
//                 new TextChunk(0, "hello hello hello"),
//                 new TextChunk(12, "hello world world"),
//                 new TextChunk(24, "world hello")
//             };
//
//             // Act
//             var result = await processor.ProcessWithStatisticsAsync(chunks);
//
//             // Assert
//             Assert.That(result.UniqueWords.Count, Is.EqualTo(2));
//             Assert.That(result.UniqueWords, Is.EquivalentTo(new[] { "hello", "world" }));
//         }
//
//         [Test]
//         public async Task ProcessWithStatisticsAsync_OrderedOutput_MaintainsAlphabeticalOrder()
//         {
//             // Arrange
//             var chunks = new[]
//             {
//                 new TextChunk(0, "zebra alpha charlie"),
//                 new TextChunk(12, "beta delta")
//             };
//
//             // Act
//             var result = await processor.ProcessWithStatisticsAsync(chunks);
//
//             // Assert
//             var expected = new[] { "alpha", "beta", "charlie", "delta", "zebra" };
//             Assert.That(result.UniqueWords, Is.EqualTo(expected));
//         }
//     }