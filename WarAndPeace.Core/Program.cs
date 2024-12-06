using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WarAndPeace.Core.IO;
using WarAndPeace.Core.Processing;

namespace WarAndPeace.Core
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // File path checking using lambdas
            var getPossiblePaths = () => new[]
            {
                "war_and_peace.txt",
                Path.Combine("..", "..", "..", "war_and_peace.txt"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "war_and_peace.txt")
            };

            var findExistingFile = (string[] paths) => paths.FirstOrDefault(File.Exists);
            var getFileSize = (string path) => new FileInfo(path).Length / 1024.0 / 1024.0;
            
            var filePath = findExistingFile(getPossiblePaths());

            if (filePath == null)
            {
                Console.WriteLine("Could not find war_and_peace.txt in any of these locations:");
                getPossiblePaths().ToList().ForEach(path => 
                    Console.WriteLine($"- {Path.GetFullPath(path)}"));
                return;
            }

            Console.WriteLine($"Found file at: {Path.GetFullPath(filePath)}");
            
            var stopwatch = new Stopwatch();
            var tokenizer = new WordTokenizer();
            
            try
            {
                Console.WriteLine($"File size: {getFileSize(filePath):F2} MB");

                stopwatch.Start();
                
                // Process file and get unique words using lambda chain
                var processFile = async () =>
                {
                    var reader = new FileChunkReader(filePath);
                    var chunks = await reader.ReadChunksAsync();
                    
                    return chunks
                        .SelectMany(chunk => tokenizer.TokenizeChunk(chunk))
                        .Distinct()
                        .OrderBy(word => word)
                        .ToList();
                };

                var uniqueWords = await processFile();
                
                stopwatch.Stop();

                // Print statistics using lambdas
                var printStats = () =>
                {
                    Console.WriteLine("\nProcessing completed!");
                    Console.WriteLine($"Unique words found: {uniqueWords.Count:N0}");
                    Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds / 1000.0:F2} seconds");
                };

                var printSampleWords = () =>
                {
                    Console.WriteLine("\nFirst 10 words in alphabetical order:");
                    uniqueWords.Take(10).ToList().ForEach(word => 
                        Console.WriteLine(word));
                };

                printStats();
                printSampleWords();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}