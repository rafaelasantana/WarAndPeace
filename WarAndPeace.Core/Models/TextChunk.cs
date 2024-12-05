namespace WarAndPeace.Core.Models
{
    public record TextChunk
    {
        
        public long Position { get; } // Byte position in the file where this chunk starts
        public string Content { get; } // Actual text content of the chunk

        public TextChunk(long position, string content)
        {
            Position = position;
            
            Content = content;
        }
    }
}