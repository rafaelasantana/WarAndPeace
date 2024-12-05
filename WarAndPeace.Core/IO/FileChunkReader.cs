using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WarAndPeace.Core.Models;

namespace WarAndPeace.Core.IO
{
    public class FileChunkReader
    {
        private readonly string filePath; // stores the path to "War and Peace" text file
        private readonly int chunkSize; // determines how many bytes to read at once 

        public FileChunkReader(string filePath, int chunkSize = 1024 * 1024) // 1MB default chunk size
        {
            this.filePath = filePath;
            this.chunkSize = chunkSize;
        }

        public async Task<IEnumerable<TextChunk>> ReadChunksAsync()
        {
            var chunks = new List<TextChunk>();

            await using var fileStream = File.OpenRead(filePath); // ensures the file is properly closed after reading
            var buffer = new byte[chunkSize];
            int bytesRead;
            long position = 0; // tracks where we are in the file

            while ((bytesRead = await fileStream.ReadAsync(buffer)) > 0)
            {
                var text = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                chunks.Add(new TextChunk(position, text));
                position += bytesRead;
            }

            return chunks;
        }
    }
}