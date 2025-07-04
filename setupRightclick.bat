@echo off
setlocal

:: Get the folder where this script is located
set "APPDIR=%~dp0"
set "HELPER=%APPDIR%TrayLink.Helper.exe"

:: Ensure the helper exists
if not exist "%HELPER%" (
    echo.
    echo ERROR: TrayLink.Helper.exe not found in "%APPDIR%"
    echo Make sure this script is in the same folder as the helper.
    pause
    exit /b 1
)

echo.
echo Registering TrayLink context menu for files and folders...
"%HELPER%" --register

echo.
echo Done. You can now right-click files or folders to 'Add to TrayLink'.
pause
