using System.Diagnostics;
using WarAndPeace.Core.IO;
using WarAndPeace.Core.Models;
using WarAndPeace.Core.Processing;

namespace WarAndPeace.Core
{
    public static class Program
    {
        // File handling functions
        private static readonly Func<string[]> GetPossiblePaths = () => new[]
        {
            "war_and_peace.txt",
            Path.Combine("..", "..", "..", "war_and_peace.txt"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "war_and_peace.txt")
        };

        private static readonly Func<string[], string?> FindExistingFile = 
            paths => paths.FirstOrDefault(File.Exists);

        private static readonly Func<string, double> GetFileSize = 
            path => new FileInfo(path).Length / 1024.0 / 1024.0;

        // Console output functions
        private static readonly Action<string[]> PrintMissingFileLocations = paths =>
        {
            Console.WriteLine("Could not find war_and_peace.txt in any of these locations:");
            paths.ToList().ForEach(path => 
                Console.WriteLine($"- {Path.GetFullPath(path)}"));
        };

        private static readonly Action<int, double> PrintStats = (wordCount, seconds) =>
        {
            Console.WriteLine("\nProcessing completed!");
            Console.WriteLine($"Unique words found: {wordCount:N0}");
            Console.WriteLine($"Time taken: {seconds:F2} seconds");
        };

        private static readonly Action<IEnumerable<string>> PrintSampleWords = words =>
        {
            Console.WriteLine("\nFirst 10 words in alphabetical order:");
            words.Take(10).ToList().ForEach(word => Console.WriteLine(word));
        };

        // File processing function
        private static readonly Func<string, Task<ProcessingResult>> ProcessFile = async filePath =>
        {
            var reader = new FileChunkReader(filePath);
            var chunks = await reader.ReadChunksAsync();
            return await ParallelProcessor.ProcessChunksAsync(chunks);
        };

        public static async Task Main(string[] args)
        {
            var paths = GetPossiblePaths();
            var filePath = FindExistingFile(paths);

            if (filePath == null)
            {
                PrintMissingFileLocations(paths);
                return;
            }

            Console.WriteLine($"Found file at: {Path.GetFullPath(filePath)}");
            Console.WriteLine($"File size: {GetFileSize(filePath):F2} MB");

            try
            {
                var stopwatch = Stopwatch.StartNew();
                
                var result = await ProcessFile(filePath);
                
                stopwatch.Stop();
                
                PrintStats(result.TotalUniqueWords, stopwatch.ElapsedMilliseconds / 1000.0);
                if (result.UniqueWords != null) PrintSampleWords(result.UniqueWords);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}