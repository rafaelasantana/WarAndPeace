using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WarAndPeace.Core.Models;

namespace WarAndPeace.Core.Processing
{
    public class WordTokenizer
    {
        private readonly Regex cleanupRegex = new(
            @"[^\w\s]|\d|_", // Added underscore to removal
            RegexOptions.Compiled
        );
        
        private readonly Func<string, string> removePunctuation;
        private readonly Func<string, string> toLowerCase;
        private readonly Func<string, bool> isValidWord;
        private readonly Func<string, string[]> splitIntoWords;
        
        public WordTokenizer()
        {
            removePunctuation = text => cleanupRegex.Replace(text, "");
            toLowerCase = text => text.ToLowerInvariant();
            // Refined word validation: at least 2 characters, only letters
            isValidWord = word => !string.IsNullOrWhiteSpace(word) 
                                  && word.Length > 1 
                                  && word.All(char.IsLetter);
            splitIntoWords = text => text.Split(
                new[] { ' ', '\n', '\r', '\t' }, 
                StringSplitOptions.RemoveEmptyEntries
            );
        }

        public IEnumerable<string> TokenizeChunk(TextChunk chunk) => 
            TokenizeText(chunk.Content);
        
        public IEnumerable<string> TokenizeText(string text) =>
            string.IsNullOrWhiteSpace(text)
                ? Enumerable.Empty<string>() 
                : splitIntoWords(text)
                    .Select(removePunctuation)
                    .Select(toLowerCase)
                    .Where(isValidWord);
    }
}