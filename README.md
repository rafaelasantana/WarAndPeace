# War and Peace Word Processing

A C# program that processes Tolstoy's "War and Peace" text file, extracting unique words and saving them to `output.txt`.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

Note: The input file `war_and_peace.txt` is included in the repository.

## Building and Running

1. Clone and build:
```bash
git clone https://github.com/rafaelasantana/WarAndPeace.git
cd WarAndPeace
dotnet build
```

2. Run tests:
```bash
dotnet test
```

3. Run program:
```bash
cd WarAndPeace
dotnet run
```

The program will process `war_and_peace.txt` and create `output.txt` containing unique words with:
- Lowercase letters only
- No punctuation or numbers
- Minimum word length of 2 characters
- No duplicates
