@echo off

echo Building solution...
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo Build failed
    exit /b %ERRORLEVEL%
)

echo Running tests...
dotnet test
if %ERRORLEVEL% NEQ 0 (
    echo Tests failed
    exit /b %ERRORLEVEL%
)

echo Running program...
cd WarAndPeace
dotnet run
if %ERRORLEVEL% NEQ 0 (
    echo Program failed
    exit /b %ERRORLEVEL%
)

echo Checking output...
if exist output.txt (
    echo Success! Check output.txt for results
    for /f %%A in ('type "output.txt" ^| find /c /v ""') do echo Found %%A unique words
) else (
    echo Error: output.txt was not created
    exit /b 1
)