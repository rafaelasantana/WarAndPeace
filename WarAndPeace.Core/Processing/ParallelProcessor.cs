using WarAndPeace.Core.Models;

namespace WarAndPeace.Core.Processing
{
    public class ParallelProcessor(WordTokenizer tokenizer, int? maxDegreeOfParallelism = null)
    {
        private readonly int _maxDegreeOfParallelism = maxDegreeOfParallelism ?? Environment.ProcessorCount;

        private async Task<IEnumerable<string>> ProcessChunksInParallelAsync(IEnumerable<TextChunk> chunks)
        {
            var chunksArray = chunks.ToArray();
            
            var tasks = new List<Task<HashSet<string>>>();
            
            var parallelOptions = new ParallelOptions 
            { 
                MaxDegreeOfParallelism = _maxDegreeOfParallelism 
            };

            await Parallel.ForEachAsync(
                chunksArray,
                parallelOptions,
                async (chunk, _) =>
                {
                    var result = await ProcessChunkTask(chunk);
                    lock (tasks)
                    {
                        tasks.Add(Task.FromResult(result));
                    }
                });

            var wordSets = await Task.WhenAll(tasks);

            return wordSets
                .Aggregate(
                    new HashSet<string>(),
                    (accumulator, current) =>
                    {
                        accumulator.UnionWith(current);
                        return accumulator;
                    }
                )
                .OrderBy(word => word);
        }

        private Task<HashSet<string>> ProcessChunkTask(TextChunk chunk) => 
            Task.Run(() => new HashSet<string>(tokenizer.TokenizeChunk(chunk)));

        public async Task<ProcessingResult> ProcessWithStatisticsAsync(IEnumerable<TextChunk> chunks)
        {
            if (chunks == null)
                throw new ArgumentNullException(nameof(chunks));
                
            try
            {
                var startTime = DateTime.UtcNow;
                var words = await ProcessChunksInParallelAsync(chunks);
                var endTime = DateTime.UtcNow;

                var uniqueWords = words.ToArray();
                return new ProcessingResult
                {
                    UniqueWords = uniqueWords.ToList(),
                    ProcessingTimeMs = (endTime - startTime).TotalMilliseconds,
                    TotalUniqueWords = uniqueWords.Length
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error processing text chunks", ex);
            }
        }
    }
}