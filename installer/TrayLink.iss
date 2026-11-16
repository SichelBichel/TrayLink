; TrayLink: Inno Setup 6 script (alternative to the Visual Studio Installer Project TrayLinkSetup).
; Modelled on installer\AutoBakCommandCenter.iss of AutoBakCommandCenter.
;
;   - the setup delivers the files of out\msi to C:\ReHoGa Interactive\TrayLink (same folder as the MSI)
;   - an update ends the running TrayLink, replaces it and (interactive setup) offers to start it again
;   - optional: start with Windows, "Add to TrayLink" in the Explorer context menu, desktop icon
;     (per user in HKCU, exactly the values TrayLink itself writes, so its own switches stay in sync)
;   - the entries and settings (%AppData%\TrayLink) of every user are never touched
;
; Build:
;   1. scripts\BuildEverythingForShipping.bat (does step 2 too when Inno Setup is installed)
;      or: powershell -ExecutionPolicy Bypass -File .\scripts\publish.ps1      (fills out\msi)
;   2. Inno Setup Compiler 6.3 or newer: open installer\TrayLink.iss, Build > Compile (Ctrl+F9)
;      or: "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\TrayLink.iss
;   Result: out\setup\TrayLink-Setup-<version>.exe
;
; Silent:
;   TrayLink-Setup-<version>.exe /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /LOG="<file>"
;   /TASKS="autostart,shellintegration" picks the options, exit code 0 = done.
;
; Note: the ReHoGa Updater (Update link, tray menu "Check for updates") installs the MSI of the update service. Ship
; one installer kind per customer: an MSI update over an Inno installation (or the other way round) gives two entries
; in Apps & features for the same folder.

#define AppName      "TrayLink"
#define AppPublisher "ReHoGa Interactive"
#define AppExe       "TrayLink.exe"
#ifndef SrcDir
  ; publish.ps1 passes /DSrcDir=<its out\msi> (also with -Output elsewhere)
  #define SrcDir     AddBackslash(SourcePath) + "..\out\msi"
#endif
#define RepoDir      AddBackslash(SourcePath) + ".."

#if !FileExists(SrcDir + "\" + AppExe)
  #error out\msi\TrayLink.exe is missing: run scripts\publish.ps1 first.
#endif
#if !FileExists(SrcDir + "\Updater.exe")
  #error out\msi\Updater.exe is missing: run scripts\publish.ps1 first.
#endif

; Version from the exe (TrayLink.csproj), "1.5.0.0" -> "1.5.0"
#define AppVersion4  GetVersionNumbersString(SrcDir + "\" + AppExe)
#define AppVersion   Copy(AppVersion4, 1, RPos(".", AppVersion4) - 1)

[Setup]
; Never change the AppId: it identifies the product for updates and uninstall.
AppId={{6C1E9E4A-3B7D-4F2A-9C55-8E0D2B7A41F3}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
VersionInfoVersion={#AppVersion4}
VersionInfoCompany={#AppPublisher}
VersionInfoProductName={#AppName}
VersionInfoDescription={#AppName} Setup
; ReHoGa convention: C:\ReHoGa Interactive\<App>, not Program Files
DefaultDirName={sd}\ReHoGa Interactive\TrayLink
DisableDirPage=yes
UsePreviousAppDir=no
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
DisableReadyPage=no
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.17763
WizardStyle=modern
SetupIconFile={#RepoDir}\TrayLink\TrayLink.ico
UninstallDisplayIcon={app}\{#AppExe}
UninstallDisplayName={#AppName}
OutputDir={#RepoDir}\out\setup
OutputBaseFilename=TrayLink-Setup-{#AppVersion}
Compression=lzma2/max
SolidCompression=yes
; TrayLink lives in the tray only (no visible main window), the Restart Manager can not close it reliably.
; It is ended in [Code] instead and started again in [Run] (interactive).
CloseApplications=no
RestartApplications=no
SetupLogging=yes
; Autostart and context menu are per user (HKCU), like TrayLink does it itself.
UsedUserAreasWarning=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "autostart";        Description: "Start TrayLink with Windows"; GroupDescription: "Options:"
Name: "shellintegration"; Description: "Add ""Add to TrayLink"" to the Explorer context menu"; GroupDescription: "Options:"
Name: "desktopicon";      Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Exactly the content of out\msi (see out\msi-files.txt): TrayLink.exe, Updater.exe, libsodium.dll.
Source: "{#SrcDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
; The shortcut takes its picture from the exe (<ApplicationIcon> of TrayLink.csproj).
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExe}"; WorkingDir: "{app}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"; WorkingDir: "{app}"; Tasks: desktopicon

[Registry]
; Same format as TrayLink\Core\Autostart.cs and TrayLink\Core\ShellIntegration.cs.
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "TrayLink"; ValueData: """{app}\{#AppExe}"""; Flags: uninsdeletevalue; Tasks: autostart

Root: HKCU; Subkey: "Software\Classes\*\shell\AddToTrayLink"; ValueType: string; ValueName: ""; ValueData: "Add to TrayLink"; Flags: uninsdeletekey; Tasks: shellintegration
Root: HKCU; Subkey: "Software\Classes\*\shell\AddToTrayLink"; ValueType: string; ValueName: "Icon"; ValueData: """{app}\{#AppExe}"",0"; Tasks: shellintegration
Root: HKCU; Subkey: "Software\Classes\*\shell\AddToTrayLink\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExe}"" --add ""%1"""; Tasks: shellintegration

Root: HKCU; Subkey: "Software\Classes\Directory\shell\AddToTrayLink"; ValueType: string; ValueName: ""; ValueData: "Add to TrayLink"; Flags: uninsdeletekey; Tasks: shellintegration
Root: HKCU; Subkey: "Software\Classes\Directory\shell\AddToTrayLink"; ValueType: string; ValueName: "Icon"; ValueData: """{app}\{#AppExe}"",0"; Tasks: shellintegration
Root: HKCU; Subkey: "Software\Classes\Directory\shell\AddToTrayLink\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExe}"" --add ""%1"""; Tasks: shellintegration

[Run]
; Start TrayLink as the signed in user (not elevated): its entries are per Windows user.
Filename: "{app}\{#AppExe}"; Description: "Start {#AppName}"; Flags: postinstall nowait skipifsilent runasoriginaluser

[UninstallDelete]
; Nothing else: %AppData%\TrayLink (actions.ini, settings.json) of every user is kept.

[Code]
const
  DownloadUrl  = 'https://dotnet.microsoft.com/download/dotnet/10.0';
  UninstallKey = 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{6C1E9E4A-3B7D-4F2A-9C55-8E0D2B7A41F3}_is1';
  RunKey       = 'Software\Microsoft\Windows\CurrentVersion\Run';
  ShellFileKey = 'Software\Classes\*\shell\AddToTrayLink';
  ShellDirKey  = 'Software\Classes\Directory\shell\AddToTrayLink';

{ True when a shared framework of major version 10 is installed (x64), e.g. Microsoft.WindowsDesktop.App\10.0.x }
function HasRuntime(const Name: String): Boolean;
var
  FindRec: TFindRec;
begin
  Result := False;
  if FindFirst(ExpandConstant('{commonpf64}\dotnet\shared\' + Name + '\10.*'), FindRec) then
  begin
    try
      repeat
        if (FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY) <> 0 then
        begin
          Result := True;
          Exit;
        end;
      until not FindNext(FindRec);
    finally
      FindClose(FindRec);
    end;
  end;
end;

procedure OpenDownloadPage();
var
  Code: Integer;
begin
  ShellExec('open', DownloadUrl, '', '', SW_SHOWNORMAL, ewNoWait, Code);
end;

{ Version rules like the MSI: same or newer installs, an older setup does not overwrite a newer installation. }
function NewerInstalled(var Installed: String): Boolean;
var
  Have, Mine: Int64;
begin
  Result := False;
  if not RegQueryStringValue(HKLM64, UninstallKey, 'DisplayVersion', Installed) then Exit;
  if StrToVersion(Installed, Have) and StrToVersion('{#AppVersion}', Mine) then
    Result := ComparePackedVersion(Have, Mine) > 0;
end;

{ TrayLink runs in the tray only, so it is ended hard. Nothing is lost: it saves on every change. }
procedure EndTrayLink();
var
  ResultCode: Integer;
begin
  Exec(ExpandConstant('{sys}\taskkill.exe'), '/F /IM {#AppExe}', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Sleep(300);
end;

function InitializeSetup(): Boolean;
var
  Installed: String;
begin
  Result := True;

  if NewerInstalled(Installed) then
  begin
    SuppressibleMsgBox(Format('{#AppName} %s is already installed. This setup (%s) is older and does not replace it.', [Installed, '{#AppVersion}']), mbError, MB_OK, IDOK);
    Result := False;
    Exit;
  end;

  { TrayLink and the Updater need the .NET Desktop Runtime 10 (x64). }
  if not HasRuntime('Microsoft.WindowsDesktop.App') then
  begin
    if SuppressibleMsgBox('The .NET Desktop Runtime 10 (x64) is missing. TrayLink cannot start without it.' + #13#10#13#10 +
      'Open the download page now? Install the "Desktop Runtime" (x64), then run this setup again.',
      mbError, MB_YESNO, IDNO) = IDYES then
      OpenDownloadPage();
    Result := False;
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  EndTrayLink();
  Result := '';
end;

function InitializeUninstall(): Boolean;
begin
  EndTrayLink();
  Result := True;
end;

function PointsToApp(const Value: String): Boolean;
begin
  Result := Pos(Lowercase(ExpandConstant('{app}')), Lowercase(Value)) > 0;
end;

procedure RemoveShellKeyIfOurs(const Key: String);
var
  Value: String;
begin
  if RegQueryStringValue(HKCU, Key + '\command', '', Value) and PointsToApp(Value) then
    RegDeleteKeyIncludingSubkeys(HKCU, Key);
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  Value: String;
begin
  if CurUninstallStep = usPostUninstall then
  begin
    { Switched on later inside TrayLink: the uninstaller does not know these entries, remove them too. }
    if RegQueryStringValue(HKCU, RunKey, 'TrayLink', Value) and PointsToApp(Value) then
      RegDeleteValue(HKCU, RunKey, 'TrayLink');
    RemoveShellKeyIfOurs(ShellFileKey);
    RemoveShellKeyIfOurs(ShellDirKey);
  end;
end;
