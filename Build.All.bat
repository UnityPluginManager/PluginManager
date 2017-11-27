@echo off

set BaseDir=%cd%

cd %BaseDir%\PluginManager.Core
dotnet build

cd %BaseDir%\PluginManager.Setup
dotnet build

echo Press any key to exit . . . 
pause > nul
