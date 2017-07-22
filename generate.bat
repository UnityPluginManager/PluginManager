@echo off

rem Generate project
Protobuild.exe --generate

rem Wait for user input and exit
echo Press any key to exit . . . 
pause > nul
exit
