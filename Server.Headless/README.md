# Legend of Mir Crystal - Headless Server

This project is the modernized, headless version of the Legend of Mir Crystal Server, fully migrated to **.NET 10.0**. It is designed to run natively on macOS, Linux and Windows without any dependencies on Windows-specific UI frameworks (like Windows Forms) or image rendering libraries.

## 🌟 Features
- **True Headless Design:** Fully decoupled from UI forms; runs directly as a console application.
- **Paced Tick Loop:** Uses the platform Stopwatch frequency and yields the CPU for short iterations. The nominal minimum interval is 1ms; actual scheduling depends on the operating system.
- **Cross-Platform Paths:** Normalizes Windows separators and resolves existing names case-insensitively without renaming game assets. Directory listings include files created by external tools.
- **Buff Save Compatibility:** Writes the existing compact bool/long/string format and reads legacy BinaryFormatter values with `System.Formats.Nrbf`, without instantiating serialized classes. Unsupported or malformed values fail explicitly instead of silently becoming null.

---

## 🛠️ Prerequisites

To run this server natively, your environment must have:
- **OS:** macOS (Apple Silicon or Intel), Linux, or Windows. No FNA, SDL, or graphical desktop is required.
- **Runtime:** [.NET 10.0 SDK or Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

---

## 🚀 Deployment & Execution

### 1. Building the Server
To compile the headless server from the source code, navigate to the root solution directory and run:
```bash
dotnet build Server.Headless/Server.Headless.csproj -c Release
```
This will compile the `Server.Headless`, `Server.Library`, and `Shared` projects and output the executable to `Build/Server/Release/net10.0/`.

### 2. Environment Setup
The server requires specific data folders to operate. Place your built executable alongside the following core directories (often found in the release `Jev` package):
- `Server.MirDB` (Your main database file)
- `Configs/` (Server settings and configurations)
- `Envir/` (Drops, NPCs, Quests, Routes, and Scripts)
- `Maps/` (Game map `.map` files)

*Keep a backup of existing account and world data before upgrading. Game data is resolved from the working directory; the CLI rejects missing database, Maps or Envir directories.*

### 3. Running the Server
You can launch the server directly using the `.NET` CLI from the directory containing your environment files:
```bash
dotnet run --project path/to/Server.Headless.csproj -c Release
```
Or run the published executable directly:
```bash
./Server.Headless
```

### 4. Graceful Shutdown
To stop the server safely, press `Ctrl + C` in the terminal or type `exit` in the console. The headless host intercepts the shutdown request to halt the game loop, securely save the database state, and commit all configurations before fully exiting.

---

## 🎮 CLI Interactive Console & `account` Command

The headless server starts an interactive console supporting auto-completion (tab-completion) and administrative commands.

### Interactive Commands
- `help` / `?`: Show the list of available commands.
- `exit`: Gracefully stop the server, saving the database state and committing configurations.
- `account`: View and edit user accounts and their nested attributes (characters, items, stats) in an OpenWrt `uci`-like format.

#### Account Path Notation & Subcommands
The `account` command works by resolving a dot-notation path, for example: `AccountID.Characters[Index].Inventory[Index].Field` or `AccountID.CharacterName.Inventory[Index].StatName`.

Available subcommands:
- **`account show`**: List all accounts, or recursively display nested attributes at a resolved path.
  ```bash
  account show asdf
  account show asdf.Honoka
  account show asdf.Honoka.Inventory
  ```
  *Note on array truncation:* When showing an object, nested arrays or lists containing more than 5 elements are truncated (omitting the remainder) to avoid terminal flooding. However, if the queried target *is* an array/list itself (e.g. `account show asdf.Honoka.Inventory`), it will display all elements without limits.
  *Note on privacy:* Sensitive fields like `Password` and `StoragePassword` are automatically displayed as `[Protected]` to prevent ANSI-corrupted/garbled text output.

- **`account get <path>`**: Get the value of a specific attribute.
  ```bash
  account get asdf.Gold
  account get asdf.Honoka.HP
  ```

- **`account set <path>=<value>`**: Modify the value of a specific attribute.
  ```bash
  account set asdf.Gold=1000
  account set asdf.Honoka.Level=15
  account set asdf.Honoka.Inventory[0].Luck=9
  ```
  *Shortcuts & Autocomplete:*
  - **Dynamic Tab Completion**: Pressing Tab on any path argument (e.g. `asdf.`) dynamically inspects the C# object hierarchy using reflection and offers completions for all nested fields, properties, character names, and `Stat` shortcuts.
  - If a segment is not found on `AccountInfo`, it will search the `Characters` list for a character matching the name (e.g., `asdf.Honoka` instead of `asdf.Characters[0]`).
  - If a segment is not found on `UserItem`, it will search the `Stat` enum and direct the set operation to the item's `AddedStats` collection.
  - Updates to online players (e.g. leveling up, item stats modifications, gold/credits changes) are automatically synchronized in real-time.

---

## ⚙️ Configuration

### Game Settings (`Configs/Setup.ini`)
When the server runs for the first time, it will automatically generate or load `Setup.ini` inside the `Configs/` directory. You can edit this file to adjust ports, experience rates, drop rates, and other environmental properties.

### Logging (`log4net.config`)
Console output and log files are managed by `log4net`. The logging configuration is located in `log4net.config` at the root of the executable directory. 
- Logs are automatically saved to `Logs/Server/`, `Logs/Chat/`, `Logs/Player/`, etc.
- By default, Linux-native forward slashes are utilized in the log paths.

---

## 🔧 Troubleshooting

- **Script Not Found Errors:** If the server console reports `Script Not Found` or `INSERT Script Not Found`, ensure that all custom scripts referenced in your `Envir/` directory exist and that their internal references (`#INCLUDE`, `#INSERT`) correctly match your file structure.
- **Failed to Load Map:** Ensure the map files referenced by the database are present in `Maps/`. Mixed-case filenames are supported.
- **Address Already in Use:** If the server fails to bind the network socket, ensure no other instance of the server is running in the background and that your configured port (default TCP `7000`) is free.

## Compatibility regression checks

```bash
dotnet run --project Tests/Server.Compatibility -c Release
```

This verifies legacy and current buff values, corrupt input handling, normalized
writes and live directory listings. Run the server from your data directory:

```bash
cd /path/to/server-data
dotnet /path/to/Crystal/Build/Server/Release/net10.0/Server.Headless.dll
```

On macOS, `Scripts/start-server.command /path/to/server-data` builds and starts the
server in that data directory. Double-clicking the script uses `Build/ServerDemo`
when that demonstration dataset has been prepared. Use `status` to inspect the
running server and `exit` to save and stop it. If port 7000 is occupied, choose a
free port in `Configs/Setup.ini` and use the same port in your client.

Legacy save decoding follows the [Microsoft NRBF migration guide](https://learn.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-migration-guide/read-nrbf-payloads).
