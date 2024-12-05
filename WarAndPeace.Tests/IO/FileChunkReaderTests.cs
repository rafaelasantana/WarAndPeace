using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WarAndPeace.Core.IO;
using WarAndPeace.Core.Models;

namespace WarAndPeace.Tests
{
    [TestFixture]
    public class FileChunkReaderTests
    {
        private string testFilePath;

        [SetUp]
        public void Setup()
        {
            // Create a temporary test file
            testFilePath = Path.GetTempFileName();
            File.WriteAllText(testFilePath, "The quick brown fox jumps over the lazy dog");
        }

        [Test]
        public async Task ReadChunksAsync_WithSmallChunkSize_ReturnsCorrectNumberOfChunks()
        {
            // Arrange
            var chunkSize = 10; // Small chunk size for testing
            var reader = new FileChunkReader(testFilePath, chunkSize);

            // Act
            var chunks = (await reader.ReadChunksAsync()).ToList();

            // Assert
            Assert.That(chunks, Is.Not.Empty);
            Assert.That(chunks.Count, Is.GreaterThan(1));
            Assert.That(chunks.First().Position, Is.EqualTo(0));
            Assert.That(chunks, Is.All.Matches<TextChunk>(c => c.Content.Length <= chunkSize));
        }

        [Test]
        public async Task ReadChunksAsync_ContentIsComplete()
        {
            // Arrange
            var originalContent = "The quick brown fox jumps over the lazy dog";
            var reader = new FileChunkReader(testFilePath, 10);

            // Act
            var chunks = await reader.ReadChunksAsync();
            var reconstructedContent = string.Concat(chunks.Select(c => c.Content));

            // Assert
            Assert.That(reconstructedContent, Is.EqualTo(originalContent));
        }

        [TearDown]
        public void Cleanup()
        {
            // Clean up the test file
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }
    }
}