# FolderSync
[![Tests](https://github.com/dmont1735/FolderSync/actions/workflows/folder-sync-tests.yml/badge.svg)](https://github.com/dmont1735/FolderSync/actions/workflows/folder-sync-tests.yml)

A one-way folder synchronization utility tool written in C# that maintains a full, identical copy of a folder at a specified replica folder.

## Requirements
- .NET 8.0 or later

## Features
- **One-way Synchronization:** Ensures replica folder mirrors source folder exactly
- **Periodical execution:** Runs automatically at user-defined intervals
- **Execution logging:** All files and folders' operations are logged to console and specified log file
- **MD5 validation:** File comparison done through sizes and hashing for efficiency and accuracy

## Installation
### Clone the repository:
```bash
git clone https://github.com/dmont1735/FolderSync.git
cd FolderSync
```
### Build the project:
```bash
dotnet build
```

## Usage
```
dotnet run -- <source_path> <replica_path> <interval_seconds> <log_file_path>
```

### Parameters:
- `source_path`: Source folder to copy.
- `replica_path`: Folder location for the synchronized copy. Will be created if it doesn't exist.
- `interval_seconds`: Synchronization interval in seconds.
- `log_file_path`:  File location for logging. Will be created if it doesn't exist.

### Example (Windows):
```bash
dotnet run -- "C:\Users\user\source" "C:\Users\user\backup" 60 "C:\Users\user\logs\sync.log"
```

### Example (Linux/macOS):
```bash
dotnet run -- "/home/user/source" "/home/user/backup" 60 "/logs/sync.log"
```

This will:
- Synchronize the source folder to the replica folder, keeping an updated copy
- Log all operations to the specified log file
- Run every 60 seconds

## How it works
1. **Initial Synchronization:** Performs synchronization of source content to replica location immediately
2. **Operations:**
    - Deletes files in replica that don't exist in source folder.
    - Copies new files from source folder to replica
    - Updates modified files in replica location
    - Creates necessary folders in replica to maintain source structure
    - Removes empty folders in replica
3. **Scheduled execution:** Performs new synchronizations on defined intervals indefinitely. User may press `Enter` at any time to stop the synchronization process.

## Logging
All operations are logged to both the console and log file specified in the input argument `log_file_path`:
```
[2026-01-10 14:36:35.820] - [INITIALIZING] - Synchronization of folder C:\Users\user\source to folder C:\Users\user\backup started
[2026-01-10 14:36:35.838] - DELETED: oldFiles\old.txt
[2026-01-10 14:36:35.869] - CREATED: newFiles
[2026-01-10 14:36:35.871] - COPIED: newFiles\newFile.txt
[2026-01-10 14:36:35.875] - DELETED: oldFiles
[2026-01-10 14:36:35.877] - [SUCCESS] - Synchronization from C:\Users\user\source to C:\Users\user\backup completed in 0.0412356s: 1 files copied, 0 overwritten, 1 deleted
[2026-01-10 14:36:35.880] - [SCHEDULED] - Synchronization will run every 60 seconds
[2026-01-10 14:36:38.072] - [ABORTED] - Synchronization stopped by user
```

## Error Handling
The most common errors are handled:
- Invalid arguments
- Missing source folder
- Access permission issues
- File I/O errors

Errors are logged to both the console and log file specified in the input argument `log_file_path`.

**Note:** Invalid arguments errors are logged only to the console for immediate user corrective action.

## Integration Tests
The project includes comprehensive integration tests that verify all synchronization scenarios using real filesystem operations.

### Test Coverage
- Copy new files to replica
- Delete files in replica not present in source
- Update files with modified content
- Skip identical files
- Create necessary folders
- Remove empty folders after deletion

### Running tests
```bash
# Run all tests
dotnet test

# Run all tests with detailed output
dotnet test --logger "console;verbosity=detailed"
```

Tests use temporary folders created in the system temp folder, ensuring:
- No impact on actual user files
- Real filesystem behavior validation
- Automatic cleanup after each test
