using WarAndPeace;

const string readFrom = "war_and_peace.txt";
const string saveTo = "output.txt";

var fileReadResult = FileOperations.ReadFile(readFrom);

var fileReadSuccess = fileReadResult.Match(_ => true, _ =>
{
    Console.WriteLine("File not found");

    return false;
}, _ =>
{
    Console.WriteLine("IO error");

    return false;
});

if (!fileReadSuccess) return;

var wordChain = WordCounting.CountWords(fileReadResult.AsT0);

var saveSuccess = WordChainOperations.SaveWords(wordChain, saveTo);

Console.WriteLine(saveSuccess ? $"{wordChain.Count:N0} words saved to {saveTo}" : "Failed to save words");

