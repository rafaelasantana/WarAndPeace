using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WarAndPeace.Core.Models;

namespace WarAndPeace.Core.Processing
{
    public static class WordTokenizer
    {
        private static readonly Regex cleanupRegex = new(
            @"[^\w\s]|\d|_",
            RegexOptions.Compiled
        );

        // Core transformation functions as lambdas
        private static readonly Func<string, string> removePunctuation = 
            text => cleanupRegex.Replace(text ?? string.Empty, "");
            
        private static readonly Func<string, string> toLowerCase = 
            text => text?.ToLowerInvariant() ?? string.Empty;
            
        private static readonly Func<string, bool> isValidWord = 
            word => !string.IsNullOrWhiteSpace(word) 
                    && word.Length > 1 
                    && word.All(char.IsLetter);
                
        private static readonly Func<string, IEnumerable<string>> splitIntoWords = 
            text => text?.Split(
                new[] { ' ', '\n', '\r', '\t' }, 
                StringSplitOptions.RemoveEmptyEntries
            ) ?? Array.Empty<string>();

        // Composition helpers
        private static readonly Func<string, IEnumerable<string>> processText =
            text => string.IsNullOrWhiteSpace(text)
                ? Enumerable.Empty<string>()
                : splitIntoWords(text)
                    .Select(removePunctuation)
                    .Select(toLowerCase)
                    .Where(isValidWord);

        // Public API
        public static IEnumerable<string> Tokenize(string text) =>
            processText(text);

        // Extension method for chunk processing if needed
        public static IEnumerable<string> TokenizeChunk(this TextChunk chunk) =>
            chunk != null
                ? processText(chunk.Content)
                : Enumerable.Empty<string>();
    }
}