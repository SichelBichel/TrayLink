@echo off
rem TrayLink: everything that ships, in one go.
rem   1. publish TrayLink + Updater to out\TrayLink-<version>\App and out\msi
rem   2. the MSI of TrayLinkSetup (when the installer project exists and is in the solution)
rem   3. the Inno Setup exe of installer\TrayLink.iss (when Inno Setup 6 is installed)
rem No admin rights needed, nothing here touches the machine.
rem Signing: BuildEverythingForShipping.bat -SignThumbprint <SHA-1 of the code signing certificate>
rem Details: scripts\publish.ps1.
setlocal
cd /d "%~dp0.."
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0publish.ps1" -BuildMsi -BuildSetup %*
set "RESULT=%ERRORLEVEL%"
echo.
if "%RESULT%"=="0" (
    echo Done. Output: "%CD%\out"
    if exist "%CD%\out" start "" explorer.exe "%CD%\out"
) else (
    echo FAILED with exit code %RESULT%. See the messages above.
)
echo.
pause
endlocal & exit /b %RESULT%
