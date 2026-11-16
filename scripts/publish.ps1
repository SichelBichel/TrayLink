<#
.SYNOPSIS
    TrayLink: builds everything that ships. No admin rights needed.

.DESCRIPTION
    Model of AutoBak Command Center's scripts\publish.ps1. TrayLink is one tray app plus the ReHoGa Updater.
    Framework dependent for win-x64, both as single file (the .NET Desktop Runtime 10 x64 is a prerequisite,
    the Inno setup checks for it).

        <Output>\TrayLink-<version>\
            App\            TrayLink.exe, Updater.exe, libsodium.dll
            files.txt       SHA-256, size and path of every file of App\ (the MSI must contain exactly these)
            version.txt     version from TrayLink\TrayLink.csproj
            *.msi           with -BuildMsi: the MSI of TrayLinkSetup\TrayLinkSetup.vdproj, plus <msi>.sha256.txt
                            (hash and size for the ReHoGa update service, product id "traylink")

        <Output>\msi\              copy of App\ under a FIXED path: the source of the files in the .vdproj and the .iss,
                                    so their paths stay the same from release to release
        <Output>\msi-files.txt      copy of files.txt
        <Output>\setup\             with -BuildSetup: TrayLink-Setup-<version>.exe (Inno Setup, installer\TrayLink.iss)
                                    plus <setup>.sha256.txt

    Run from anywhere (scripts\BuildEverythingForShipping.bat does it with -BuildMsi -BuildSetup):
        powershell -ExecutionPolicy Bypass -File .\scripts\publish.ps1
        powershell -ExecutionPolicy Bypass -File .\scripts\publish.ps1 -BuildMsi -BuildSetup
        powershell -ExecutionPolicy Bypass -File .\scripts\publish.ps1 -SignThumbprint <SHA-1 of the code signing certificate>

.PARAMETER Output         Base folder (default: <repo>\out). The version folder is replaced.
.PARAMETER RunTests       Accepted for the same command line as the other ReHoGa apps. TrayLink has no test project yet,
                          so this only prints a note.
.PARAMETER BuildMsi       After the publish, build the Visual Studio Installer Project with devenv.com (needs Visual
                          Studio with the "Microsoft Visual Studio Installer Projects" extension). No .vdproj = skipped
                          with a note.
.PARAMETER BuildSetup     After the publish, compile installer\TrayLink.iss with ISCC.exe (Inno Setup 6.3 or newer).
                          Inno Setup not installed = skipped with a note.
.PARAMETER SignThumbprint Code signing certificate (CurrentUser\My or LocalMachine\My): signs our own binaries before
                          the file list and the MSI are made, and the MSI and the setup after they were built
                          (signtool.exe from the Windows SDK). libsodium.dll stays as it is. Empty = not signed.
.PARAMETER TimestampUrl   RFC 3161 timestamp server for signing.
#>
param(
    [string]$Output = '',
    [switch]$RunTests,
    [switch]$BuildMsi,
    [switch]$BuildSetup,
    [string]$SignThumbprint = '',
    [string]$TimestampUrl = 'http://timestamp.digicert.com'
)

$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
if (-not $Output) { $Output = Join-Path $repo 'out' }

function Invoke-Native {
    param([string]$Exe, [string[]]$Arguments)
    & $Exe @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Exe $($Arguments -join ' ') failed ($LASTEXITCODE)" }
}

# Version: TrayLink has no Directory.Build.props, the version lives in the csproj.
$csproj = Join-Path $repo 'TrayLink\TrayLink.csproj'
[xml]$props = Get-Content -Raw -Path $csproj
$version = $null
foreach ($name in 'InformationalVersion', 'Version', 'AssemblyVersion') {
    $version = @($props.Project.PropertyGroup | ForEach-Object { $_.$name } | Where-Object { $_ })[0]
    if ($version) { break }
}
if (-not $version) { throw "No <InformationalVersion>, <Version> or <AssemblyVersion> in $csproj." }
$version = $version.Trim()

$root = Join-Path $Output "TrayLink-$version"
$app = Join-Path $root 'App'
Write-Host "TrayLink $version -> $root"

# 1. Tests (optional). No test project yet.
if ($RunTests) {
    Write-Host '== Unit tests'
    Write-Warning 'TrayLink has no test project: tests skipped.'
}

# 2. Publish the app and the Updater into one folder (Update.cs starts Updater.exe next to TrayLink.exe).
#    BuildUpdater=false: the csproj target would publish the Updater a second time into updater_pub.
if (Test-Path $root) { Remove-Item -Path $root -Recurse -Force }
Write-Host '== Publish'
Invoke-Native dotnet @('publish', $csproj,
    '-c', 'Release', '-r', 'win-x64', '--self-contained', 'false',
    '/p:PublishSingleFile=true', '/p:PublishTrimmed=false', '/p:BuildUpdater=false',
    '-o', $app, '-nologo')
Invoke-Native dotnet @('publish', (Join-Path $repo 'Updater\Updater.csproj'),
    '-c', 'Release', '-r', 'win-x64', '--self-contained', 'false',
    '/p:PublishSingleFile=true', '/p:PublishTrimmed=false',
    '-o', $app, '-nologo')

# Debug symbols do not ship.
Get-ChildItem -Path $app -Recurse -Filter '*.pdb' | Remove-Item -Force

# A release without these is broken: the tray app, the Updater, the native library of the update signature check.
foreach ($required in 'TrayLink.exe', 'Updater.exe') {
    if (-not (Test-Path (Join-Path $app $required))) { throw "$required is missing in $app" }
}
if (-not (Test-Path (Join-Path $app 'libsodium.dll')) -and -not (Test-Path (Join-Path $app 'runtimes\win-x64\native\libsodium.dll'))) {
    throw "libsodium.dll is missing in $app (NSec.Cryptography package of TrayLink?)"
}

# The .vdproj lists its files one by one. Anything new here must be added there too (the .iss takes everything).
$expected = @('TrayLink.exe', 'Updater.exe', 'libsodium.dll')
$extra = @(Get-ChildItem -Path $app -Recurse -File | ForEach-Object { $_.FullName.Substring($app.Length + 1) } |
    Where-Object { $expected -notcontains $_ })
if ($extra.Count -gt 0) {
    Write-Warning ("Files in App\ that TrayLinkSetup.vdproj does not list (add them there, or the MSI misses them): " +
        ($extra -join ', '))
}

# 3. Signing (optional), before the file list and the MSI (both must hold the signed files).
$signtool = $null
$thumb = ''
if ($SignThumbprint) {
    $thumb = ($SignThumbprint -replace '[\s:-]', '').ToUpperInvariant()
    $cert = Get-ChildItem Cert:\CurrentUser\My, Cert:\LocalMachine\My | Where-Object { $_.Thumbprint -eq $thumb } | Select-Object -First 1
    if (-not $cert) { throw "Code signing certificate $thumb not found in CurrentUser\My or LocalMachine\My." }
    $signtool = Get-ChildItem "${env:ProgramFiles(x86)}\Windows Kits\10\bin\*\x64\signtool.exe" -ErrorAction SilentlyContinue |
        Sort-Object FullName -Descending | Select-Object -First 1
    if (-not $signtool) { throw 'signtool.exe not found (Windows SDK, signing tools).' }
    # Only our own binaries; libsodium keeps its own signature (or none).
    $ours = Get-ChildItem -Path $app -Recurse -Include 'TrayLink.exe', 'Updater.exe'
    foreach ($file in $ours) {
        Invoke-Native $signtool.FullName @('sign', '/sha1', $thumb, '/fd', 'SHA256', '/tr', $TimestampUrl, '/td', 'SHA256', '/q', $file.FullName)
    }
    Write-Host "Signed $(@($ours).Count) binaries with $($cert.Subject)."
}

# 4. File list and the fixed MSI source folder.
# "<SHA-256>  <size>  <relative path>" for every file below $folder (the same format as AutoBak Server / CC).
function Get-FileList([string]$folder) {
    $full = (Resolve-Path $folder).Path.TrimEnd('\')
    foreach ($file in Get-ChildItem -Path $full -Recurse -File | Sort-Object FullName) {
        $relative = $file.FullName.Substring($full.Length + 1)
        $hash = (Get-FileHash -Path $file.FullName -Algorithm SHA256).Hash
        '{0}  {1,12}  {2}' -f $hash, $file.Length, $relative
    }
}

$lines = @(Get-FileList $app)
Set-Content -Path (Join-Path $root 'files.txt') -Value $lines -Encoding UTF8
Set-Content -Path (Join-Path $root 'version.txt') -Value $version -Encoding ASCII

$fixed = Join-Path $Output 'msi'
if (Test-Path $fixed) { Remove-Item -Path $fixed -Recurse -Force }
Copy-Item -Path $app -Destination $fixed -Recurse
Copy-Item -Path (Join-Path $root 'files.txt') -Destination (Join-Path $Output 'msi-files.txt') -Force
Write-Host "MSI content: $fixed ($($lines.Count) files, list in $(Join-Path $Output 'msi-files.txt'))"

# 5. MSI (optional): the Visual Studio Installer Project, built with devenv.com through the solution.
if ($BuildMsi) {
    Write-Host '== MSI'
    $vdproj = Join-Path $repo 'TrayLinkSetup\TrayLinkSetup.vdproj'
    $sln = Join-Path $repo 'TrayLink.sln'
    $vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (-not (Test-Path $vdproj)) {
        Write-Warning "No installer project ($vdproj): MSI skipped."
    } elseif (-not (Select-String -Path $sln -Pattern 'TrayLinkSetup.vdproj' -SimpleMatch -Quiet)) {
        Write-Warning 'The installer project is not part of TrayLink.sln: MSI skipped. Add it to the solution in Visual Studio.'
    } elseif (-not (Test-Path $vswhere)) {
        Write-Warning 'vswhere.exe not found (Visual Studio not installed?): MSI skipped. Build TrayLinkSetup in Visual Studio.'
    } else {
        # The MSI version must match, otherwise the ReHoGa Updater offers the same update again and again.
        $vdVersion = (Select-String -Path $vdproj -Pattern '"ProductVersion" = "8:([^"]+)"' |
            Select-Object -First 1).Matches.Groups[1].Value
        if ($vdVersion -ne $version) {
            throw ("TrayLinkSetup ProductVersion is '$vdVersion', the app is '$version'. Set ProductVersion of TrayLinkSetup " +
                "to $version in Visual Studio (Properties window) and answer YES when it asks to change the ProductCode. " +
                "Never change the UpgradeCode.")
        }

        $devenv = & $vswhere -latest -prerelease -products * -property productPath
        if (-not $devenv) { throw 'Visual Studio not found by vswhere.' }
        $devenvCom = [System.IO.Path]::ChangeExtension($devenv, '.com')
        $before = Get-Date
        & $devenvCom $sln /Build 'Release' /Project 'TrayLinkSetup'
        if ($LASTEXITCODE -ne 0) {
            throw ("devenv failed ($LASTEXITCODE). A command line build of an installer project often needs this once: " +
                "run DisableOutOfProcBuild.exe from <Visual Studio>\Common7\IDE\CommonExtensions\Microsoft\VSI\DisableOutOfProcBuild, " +
                "or build TrayLinkSetup in Visual Studio.")
        }
        $msiFile = Get-ChildItem -Path (Join-Path $repo 'TrayLinkSetup') -Recurse -Filter '*.msi' |
            Where-Object { $_.LastWriteTime -ge $before } | Sort-Object LastWriteTime -Descending | Select-Object -First 1
        if (-not $msiFile) { throw 'devenv ran, but no new .msi was found below TrayLinkSetup.' }
        $target = Join-Path $root "TrayLink-$version.msi"
        Copy-Item $msiFile.FullName $target -Force
        if ($signtool) {
            Invoke-Native $signtool.FullName @('sign', '/sha1', $thumb, '/fd', 'SHA256', '/tr', $TimestampUrl, '/td', 'SHA256', '/q', $target)
        }
        $msiHash = (Get-FileHash -Path $target -Algorithm SHA256).Hash.ToLowerInvariant()
        $msiSize = (Get-Item $target).Length
        Set-Content -Path "$target.sha256.txt" -Value @("sha256 $msiHash", "size_bytes $msiSize", "version $version") -Encoding ASCII
        Write-Host "MSI: $target ($([math]::Round($msiSize / 1MB, 1)) MB, sha256 $msiHash)"
    }
}

# 6. Inno Setup (optional): the alternative installer, same folder and content as the MSI.
if ($BuildSetup) {
    Write-Host '== Inno Setup'
    $iss = Join-Path $repo 'installer\TrayLink.iss'
    $iscc = @("${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe", "$env:ProgramFiles\Inno Setup 6\ISCC.exe", "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe") |
        Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $iscc) {
        $cmd = Get-Command ISCC.exe -ErrorAction SilentlyContinue
        if ($cmd) { $iscc = $cmd.Source }
    }
    if (-not (Test-Path $iss)) {
        Write-Warning "No Inno script ($iss): setup skipped."
    } elseif (-not $iscc) {
        Write-Warning 'Inno Setup 6 (ISCC.exe) not found: setup skipped. Install it from https://jrsoftware.org/isinfo.php or compile installer\TrayLink.iss by hand.'
    } else {
        $setupDir = Join-Path $Output 'setup'
        # /D: the source folder of this run (default in the .iss: <repo>\out\msi), /O: the output folder.
        Invoke-Native $iscc @('/Q', "/DSrcDir=$fixed", "/O$setupDir", $iss)
        $setup = Join-Path $setupDir "TrayLink-Setup-$version.exe"
        if (-not (Test-Path $setup)) { throw "ISCC ran, but $setup is missing." }
        if ($signtool) {
            Invoke-Native $signtool.FullName @('sign', '/sha1', $thumb, '/fd', 'SHA256', '/tr', $TimestampUrl, '/td', 'SHA256', '/q', $setup)
        }
        $setupHash = (Get-FileHash -Path $setup -Algorithm SHA256).Hash.ToLowerInvariant()
        $setupSize = (Get-Item $setup).Length
        Set-Content -Path "$setup.sha256.txt" -Value @("sha256 $setupHash", "size_bytes $setupSize", "version $version") -Encoding ASCII
        Write-Host "Setup: $setup ($([math]::Round($setupSize / 1MB, 1)) MB, sha256 $setupHash)"
    }
}

Write-Host "Done: TrayLink $version, $($lines.Count) files in $app" -ForegroundColor Green
