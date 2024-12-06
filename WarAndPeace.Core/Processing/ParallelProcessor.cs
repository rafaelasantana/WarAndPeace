using System.Collections.Concurrent;
using System.Collections.Immutable;
using WarAndPeace.Core.Models;

namespace WarAndPeace.Core.Processing
{
    public static class ParallelProcessor
    {
        // Configuration function
        private static readonly Func<int?, ParallelOptions> CreateParallelOptions =
            maxDegreeOfParallelism => new ParallelOptions 
            { 
                MaxDegreeOfParallelism = maxDegreeOfParallelism ?? Environment.ProcessorCount 
            };

        // Core processing functions
        private static readonly Func<TextChunk, IEnumerable<string>> ProcessChunk =
            chunk => WordTokenizer.Tokenize(chunk.Content);

        private static readonly Func<IEnumerable<string>, ImmutableHashSet<string>> ToImmutableSet =
            words => words.ToImmutableHashSet();

        // Timing function
        private static readonly Func<DateTime, DateTime, double> CalculateProcessingTime =
            (start, end) => (end - start).TotalMilliseconds;

        // Main processing pipeline
        public static async Task<ProcessingResult> ProcessChunksAsync(
            IEnumerable<TextChunk> chunks,
            int? maxDegreeOfParallelism = null)
        {
            var startTime = DateTime.UtcNow;
            
            try
            {
                var parallelOptions = CreateParallelOptions(maxDegreeOfParallelism);
                var chunksArray = chunks.ToImmutableArray();

                // Process chunks in parallel and collect results
                var processedSets = await ProcessChunksParallel(chunksArray, parallelOptions);
                
                // Combine results
                var uniqueWords = CombineAndSortResults(processedSets);
                
                var endTime = DateTime.UtcNow;
                
                return new ProcessingResult
                {
                    UniqueWords = uniqueWords,
                    ProcessingTimeMs = CalculateProcessingTime(startTime, endTime),
                    TotalUniqueWords = uniqueWords.Count
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error processing text chunks", ex);
            }
        }

        // Helper functions for parallel processing
        private static async Task<ImmutableArray<ImmutableHashSet<string>>> ProcessChunksParallel(
            ImmutableArray<TextChunk> chunks,
            ParallelOptions parallelOptions)
        {
            var results = new ConcurrentBag<ImmutableHashSet<string>>();

            await Parallel.ForEachAsync(
                chunks,
                parallelOptions,
                async (chunk, _) =>
                {
                    var processed = await Task.Run(() => 
                        ToImmutableSet(ProcessChunk(chunk)));
                    results.Add(processed);
                });

            return [..results];
        }

        private static ImmutableList<string> CombineAndSortResults(
            ImmutableArray<ImmutableHashSet<string>> processedSets) =>
            processedSets
                .Aggregate(
                    ImmutableHashSet<string>.Empty,
                    (acc, curr) => acc.Union(curr))
                .OrderBy(word => word)
                .ToImmutableList();
    }
}