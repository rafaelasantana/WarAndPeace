using OneOf;

namespace WarAndPeace;

public readonly struct FileNotFound;

public readonly struct IOError;

public static class FileOperations
{
    public static OneOf<List<string>, FileNotFound, IOError> ReadFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) return new FileNotFound();

            var lines = File.ReadLines(filePath).ToList();

            return OneOf<List<string>, FileNotFound, IOError>.FromT0(lines);
        }
        catch (Exception)
        {
            return new IOError();
        }
    }
}