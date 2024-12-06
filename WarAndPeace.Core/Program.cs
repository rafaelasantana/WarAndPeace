using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WarAndPeace.Core.IO;
using WarAndPeace.Core.Models;
using WarAndPeace.Core.Processing;

namespace WarAndPeace.Core
{
    public static class Program
    {
        // File handling functions
        private static readonly Func<string[]> getPossiblePaths = () => new[]
        {
            "war_and_peace.txt",
            Path.Combine("..", "..", "..", "war_and_peace.txt"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "war_and_peace.txt")
        };

        private static readonly Func<string[], string> findExistingFile = 
            paths => paths.FirstOrDefault(File.Exists);

        private static readonly Func<string, double> getFileSize = 
            path => new FileInfo(path).Length / 1024.0 / 1024.0;

        // Console output functions
        private static readonly Action<string[]> printMissingFileLocations = paths =>
        {
            Console.WriteLine("Could not find war_and_peace.txt in any of these locations:");
            paths.ToList().ForEach(path => 
                Console.WriteLine($"- {Path.GetFullPath(path)}"));
        };

        private static readonly Action<int, double> printStats = (wordCount, seconds) =>
        {
            Console.WriteLine("\nProcessing completed!");
            Console.WriteLine($"Unique words found: {wordCount:N0}");
            Console.WriteLine($"Time taken: {seconds:F2} seconds");
        };

        private static readonly Action<IEnumerable<string>> printSampleWords = words =>
        {
            Console.WriteLine("\nFirst 10 words in alphabetical order:");
            words.Take(10).ToList().ForEach(word => Console.WriteLine(word));
        };

        // File processing function
        private static readonly Func<string, Task<ProcessingResult>> processFile = async filePath =>
        {
            var reader = new FileChunkReader(filePath);
            var chunks = await reader.ReadChunksAsync();
            return await ParallelProcessor.ProcessChunksAsync(chunks);
        };

        public static async Task Main(string[] args)
        {
            var paths = getPossiblePaths();
            var filePath = findExistingFile(paths);

            if (filePath == null)
            {
                printMissingFileLocations(paths);
                return;
            }

            Console.WriteLine($"Found file at: {Path.GetFullPath(filePath)}");
            Console.WriteLine($"File size: {getFileSize(filePath):F2} MB");

            try
            {
                var stopwatch = Stopwatch.StartNew();
                
                var result = await processFile(filePath);
                
                stopwatch.Stop();
                
                printStats(result.TotalUniqueWords, stopwatch.ElapsedMilliseconds / 1000.0);
                printSampleWords(result.UniqueWords);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}