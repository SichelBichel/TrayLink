Imports System

Dim shell, helperPath, result
Set shell = CreateObject("WScript.Shell")

' Get the current script directory
Dim fso, scriptPath
Set fso = CreateObject("Scripting.FileSystemObject")
scriptPath = fso.GetParentFolderName(WScript.ScriptFullName)

' Path to TrayLink.Helper.exe
helperPath = """" & scriptPath & "\TrayLink.Helper.exe"" --register"

' Run TrayLink.Helper.exe silently
result = shell.Run(helperPath, 0, True)  ' 0 = hidden window, True = wait for finish

Set shell = Nothing
Set fso = Nothing
