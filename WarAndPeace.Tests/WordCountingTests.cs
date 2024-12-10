namespace WarAndPeace.Tests;

[TestFixture]
public class WordCountingTests
{
    [Test]
    public async Task CountWords_Introduction()
    {
        var lines = new List<string>
                    {
                        "The Project Gutenberg EBook of War and Peace, by Leo Tolstoy",
                        "(#9 in our series by Leo Tolstoy)",
                        "Copyright laws are changing all over the world. Be sure to check the",
                        "copyright laws for your country before downloading or redistributing",
                        "this or any other Project Gutenberg eBook."
                    };

        var result = WordCounting.CountWords(lines);

        await Verify(result);
    }

    [Test]
    public async Task CountWords_EmptyInput()
    {
        var result = WordCounting.CountWords([]);

        await Verify(result);
    }

    [Test]
    public async Task CountWords_WithExcessivePunctuation()
    {
        var lines = new List<string>
                    {
                        "Title: War and Peace",
                        "",
                        "Author: Leo Tolstoy",
                        "",
                        "Translator: Louise and Aylmer Maude",
                        "",
                        "Release Date: April, 2001  [EBook #2600]",
                        "[This file was first posted on October 7, 2003]",
                        "[Most recently updated: May 21, 2006]",
                        "",
                        "Edition: 11",
                        "",
                        "Language: English",
                        "",
                        "Character set encoding: US-ASCII",
                        "",
                        "*** START OF THE PROJECT GUTENBERG EBOOK, WAR AND PEACE ***"
                    };

        var result = WordCounting.CountWords(lines);

        await Verify(result);
    }
}