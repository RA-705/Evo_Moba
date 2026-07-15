@echo off
echo ============================================
echo   DotaX Zero Friction - Server Builder
echo ============================================
echo.

set CONFIG=Release
set RUNTIME=win-x64
set OUTPUT=.\publish

echo [1/3] Cleaning old build...
if exist %OUTPUT% rmdir /s /q %OUTPUT%

echo [2/3] Building %CONFIG% for %RUNTIME%...
dotnet publish -c %CONFIG% -r %RUNTIME% --self-contained -o %OUTPUT% -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo [3/3] Copying config files...
if exist .\evoconfig.json copy .\evoconfig.json %OUTPUT%\evoconfig.json
if not exist %OUTPUT%\evoconfig.json (
    echo {} > %OUTPUT%\evoconfig.json
)

echo.
echo ============================================
echo   Build complete!
echo   Output: %OUTPUT%\Evo.MOBA.exe
echo ============================================
echo.
echo Run with: %OUTPUT%\Evo.MOBA.exe
pause
