using System.Collections.Immutable;
using WarAndPeace.Core.Models;

namespace WarAndPeace.Core.Processing
{
    public static class ParallelProcessor
    {
        // Pure configuration functions
        private static readonly Func<int?, int> DetermineParallelism =
            maxDegree => maxDegree ?? Environment.ProcessorCount;
            
        private static readonly Func<int, ParallelOptions> CreateParallelOptions =
            parallelism => new ParallelOptions { MaxDegreeOfParallelism = parallelism };

        // Pure data transformation functions
        private static readonly Func<TextChunk, Task<ImmutableHashSet<string>>> ProcessChunkAsync =
            async chunk => (await Task.Run(() => 
                WordTokenizer.Tokenize(chunk.Content))).ToImmutableHashSet();

        private static readonly Func<ImmutableArray<ImmutableHashSet<string>>, ImmutableList<string>> CombineResults =
            sets => sets
                .Aggregate(
                    ImmutableHashSet<string>.Empty,
                    (acc, curr) => acc.Union(curr))
                .OrderBy(word => word)
                .ToImmutableList();

        // Time measurement as pure transformation
        private readonly record struct TimeFrame(DateTime Start, DateTime End)
        {
            public double ElapsedMilliseconds => (End - Start).TotalMilliseconds;
        }

        private static readonly Func<TimeFrame, ImmutableList<string>, ProcessingResult> CreateResult =
            (timeFrame, words) => new ProcessingResult
            {
                UniqueWords = words,
                ProcessingTimeMs = timeFrame.ElapsedMilliseconds,
                TotalUniqueWords = words.Count
            };

        // Main processing pipeline
        public static async Task<ProcessingResult> ProcessChunksAsync(
            IEnumerable<TextChunk>? chunks,
            int? maxDegreeOfParallelism = null)
        {
            if (chunks is null) return CreateEmptyResult();

            var timeFrame = new TimeFrame(DateTime.UtcNow, DateTime.MinValue);
            
            try
            {
                var parallelism = DetermineParallelism(maxDegreeOfParallelism);
                CreateParallelOptions(parallelism);
                var immutableChunks = chunks.ToImmutableArray();

                var processedSets = await ProcessChunksParallelAsync(immutableChunks);
                var uniqueWords = CombineResults(processedSets);
                
                timeFrame = timeFrame with { End = DateTime.UtcNow };
                
                return CreateResult(timeFrame, uniqueWords);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error processing text chunks", ex);
            }
        }

        // Helper functions
        private static async Task<ImmutableArray<ImmutableHashSet<string>>> ProcessChunksParallelAsync(
            ImmutableArray<TextChunk> chunks)
        {
            var processingTasks = chunks
                .Select(chunk => ProcessChunkAsync(chunk))
                .ToImmutableArray();

            var results = await Task.WhenAll(processingTasks);
            return [..results];
        }

        private static ProcessingResult CreateEmptyResult() => new()
        {
            UniqueWords = ImmutableList<string>.Empty,
            ProcessingTimeMs = 0,
            TotalUniqueWords = 0
        };
    }
}