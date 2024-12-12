#!/bin/bash

# Exit on any error
set -e

echo "Building solution..."
dotnet build

echo "Running tests..."
dotnet test

echo "Running program..."
cd WarAndPeace
dotnet run

echo "Checking output..."
if [ -f "output.txt" ]; then
    echo "Success! Check output.txt for results"
    wc -l output.txt | cut -d' ' -f1 | xargs -I {} echo "Found {} unique words"
else
    echo "Error: output.txt was not created"
    exit 1
fi