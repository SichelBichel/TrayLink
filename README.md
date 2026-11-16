# TrayLink

Small tray tool by ReHoGa Interactive: left click on the tray icon opens a list of cards (URLs, files, folders) that start with one click.

## Usage

- **Left click** on the tray icon: open or close the popup.
- **Right click** on the tray icon: Open, Start with Windows, Check for updates, Exit.
- **+** next to the search: add an entry.
- **Edit** (pencil): name, type (URL / FILE / PATH), path and **position** (number, 1 = top).
- **Reorder**: drag a card with the mouse (anywhere except the buttons) or use the arrow buttons. Reordering is disabled while the search filter is active.
- **Drop files or folders** from Explorer into the list to add them.
- **Shell Integration** button: adds or removes "Add to TrayLink" in the Explorer context menu (per user, no admin rights needed). On Windows 11 it is under "Show more options".
- `%VAR%` environment variables are expanded in paths, for example `%USERPROFILE%\Downloads`.

## Files

Everything is stored per user in `%APPDATA%\TrayLink`:

| File | Content |
|---|---|
| `actions.ini` | the entries, the section order is the card order |
| `settings.json` | "Close on focus loss" |

An `actions.ini` from older versions (next to the exe) is copied there on first start. Changes to `actions.ini` from outside (editor, context menu) are picked up automatically.

## Command line

| Call | Effect |
|---|---|
| `TrayLink.exe` | start (a second start only opens the popup of the running instance) |
| `TrayLink.exe --add "<path>"` | add a file or folder and exit (used by the context menu) |
| `TrayLink.exe --register` / `--unregister` | add / remove the context menu entry |

## Build and release

Solution: `TrayLink.sln` with

- `TrayLink` (WinForms, .NET 10)
- `Updater` (ReHoGa Updater, built and copied next to `TrayLink.exe` by the `BuildUpdater` target)
- `TrayLinkSetup` (MSI, Visual Studio Installer Projects)

Update flow is the same as in the other ReHoGa apps (see `ReHoGaInteractive/Updater-Client`): product id `traylink`, channel `stable`, server `https://updates.rehoga.ch`.

Release checklist:

1. Raise the version in `TrayLink/TrayLink.csproj` (`AssemblyVersion`, `FileVersion`, `InformationalVersion`).
2. Raise `ProductVersion` in the setup project to the same value. Visual Studio then asks to change the ProductCode: answer **Yes**. Never change the `UpgradeCode`.
3. Build the MSI and publish it for product `traylink` on the update server.
