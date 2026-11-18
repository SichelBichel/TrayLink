# TrayLink

Small tray tool by ReHoGa Interactive: left click on the tray icon opens a list of cards (URLs, files, folders) that start with one click.

## Usage

The popup has two tabs: **Links** (the cards that start URLs, files and folders) and **Desktops** (saved desktop layouts). "Options" in the header holds: Add link, Open / Reload `actions.ini`, Close on focus loss, Explorer context menu, Start with Windows, Check for updates.

- **Left click** on the tray icon: open or close the popup.
- **Right click** on the tray icon: Open, Start with Windows, Check for updates, Exit.
- **+** next to the search (Links tab): add a link.
- **Edit** (pencil): name, type (URL / FILE / PATH), path and **position** (number, 1 = top).
- **Reorder**: drag a card with the mouse (anywhere except the buttons) or use the arrow buttons. Reordering is disabled while the search filter is active.
- **Drop files or folders** from Explorer into the list to add them.
- **Options > "Add to TrayLink" in the Explorer menu**: adds or removes the entry in the Explorer context menu (per user, no admin rights needed). On Windows 11 it is under "Show more options".
- `%VAR%` environment variables are expanded in paths, for example `%USERPROFILE%\Downloads`.

## Desktop layouts

Saves every open window with its monitor, position and state (normal, maximized, minimized) and brings it back later or once per Windows sign in.

- **Save**: "Save current desktop..." in the Desktops tab, or tray menu "Desktop layouts". Several named layouts are possible.
- **Restore**: "Restore" on the layout card, tray menu "Desktop layouts" > "Restore ...", or "Restore now" in the manager. Windows that are open already are placed, missing apps are started and placed as soon as their window appears (and placed again 3 s later, for apps that move themselves).
- **At Windows start**: "Restore this layout when Windows starts" in the manager (needs "Start with Windows"). Runs once per sign in, 10 s after TrayLink started.

How a window comes back:

| Window | Started with |
|---|---|
| File Explorer | its folder |
| Program started with a file or arguments | the same arguments (shown in "Start with", editable, for example a DAW project) |
| Store app, Windows Terminal | its app id |
| Discord, Slack and other Squirrel apps | their `Update.exe` (the app folder changes with every update) |
| Browser | started once, its own session restore brings the tabs, TrayLink places the windows |
| Anything else that can not be started (system windows, no access) | position only, when it is open |

Per window in the manager: Use (on/off), Start (Auto / Position only), Start with, Delay in seconds. "Update from desktop" takes the open windows again and keeps these settings.

Not possible: windows of programs that run as administrator can not be moved by TrayLink (Windows blocks it). Browser tabs come from the browser itself ("Continue where you left off"). With Explorer tabs (Windows 11) only the first tab is opened again.

## Files

Everything is stored per user in `%APPDATA%\TrayLink`:

| File | Content |
|---|---|
| `actions.ini` | the entries, the section order is the card order |
| `settings.json` | "Close on focus loss", layout restored at Windows start |
| `layouts\*.json` | the desktop layouts |

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
