# Requirements Document

## Introduction

The Photo Management Utility (foto_list) is a cross-platform command-line tool and library designed to help users locate, compare, and manage photo files across file systems. The system provides reliable file discovery and movement utilities for photo organization workflows, offering a well-tested reference implementation that validates file system operations across Windows, Linux, and macOS platforms. The tool is specifically designed for CI-friendly operations with fast, hermetic tests and minimal external dependencies.

## Glossary

- **Photo_Management_System**: The complete foto_list application including CLI and library components
- **File_Discovery_Engine**: The component responsible for recursively scanning directories and cataloging files
- **Baseline_List**: A text file containing a reference list of photo filenames from a source directory
- **Target_Directory**: The directory being compared against or cleaned based on the baseline list
- **Clean_Operation**: The process of moving files not present in the baseline list to a "removed" subdirectory
- **Diff_Report**: A comparison report showing files missing from either baseline or target locations
- **Cross_Platform_Support**: Compatibility across Windows, Linux, and macOS operating systems
- **FotoManager**: The core business logic component implementing IFotoManger interface
- **FileSystem_Abstraction**: The IFileSystem interface that abstracts file operations for testability
- **Console_Application**: The Program.cs entry point that handles command-line arguments and user interaction

## Requirements

### Requirement 1

**User Story:** As a photo organizer, I want to create a baseline list of all photos in a directory structure, so that I can track and manage my photo collection.

#### Acceptance Criteria

1. WHEN the user runs the Console_Application without parameters, THE Photo_Management_System SHALL prompt for a photo folder path and create a default list file
2. WHEN a valid directory path is provided, THE File_Discovery_Engine SHALL recursively scan all subdirectories for files with any extension
3. THE Photo_Management_System SHALL extract filenames without extensions, exclude hidden files starting with dot, and store unique names in a sorted collection
4. THE Photo_Management_System SHALL create a text file named "foto_list.txt" on the desktop containing all discovered photo filenames
5. THE Photo_Management_System SHALL display the total count of photos processed and the output file location to the console

### Requirement 2

**User Story:** As a photo organizer, I want to compare two photo collections, so that I can identify missing or extra files between locations.

#### Acceptance Criteria

1. WHEN the user provides the --compare parameter with a baseline list file path, THE Console_Application SHALL prompt for a target directory path
2. WHEN both baseline list and target directory are valid, THE FotoManager SHALL read the baseline list and scan the target directory for all files
3. THE Photo_Management_System SHALL generate two Diff_Reports: "file_missing_baseline.txt" for files in target but not baseline, and "file_missing_target.txt" for files in baseline but not target
4. THE Photo_Management_System SHALL save both diff reports as separate text files on the desktop with second-based timestamps to prevent overwrites
5. THE Photo_Management_System SHALL return a combined status message indicating the success of both report generations

### Requirement 3

**User Story:** As a photo organizer, I want to clean a directory based on a baseline list, so that I can remove unwanted files while preserving important photos.

#### Acceptance Criteria

1. WHEN the user provides the --clean or --path parameter with a baseline list file path, THE Console_Application SHALL prompt for a target directory path
2. WHEN both baseline list and target directory are valid, THE FotoManager SHALL identify files whose names without extensions are not present in the baseline list
3. THE Clean_Operation SHALL create a "removed" subdirectory within each processed directory if it does not already exist
4. THE Clean_Operation SHALL move all non-baseline files to the appropriate "removed" subdirectory while preserving directory structure
5. THE Photo_Management_System SHALL generate a report named "removed_foto_list.txt" listing all moved files and save it to the desktop

### Requirement 4

**User Story:** As a developer, I want the system to work consistently across different operating systems, so that users can rely on the same functionality regardless of their platform.

#### Acceptance Criteria

1. THE FileSystem_Abstraction SHALL normalize file paths using Path.DirectorySeparatorChar and replace both forward and backward slashes appropriately
2. THE Photo_Management_System SHALL handle case-sensitive and case-insensitive file systems by using consistent path normalization in the Console_Application
3. THE FileSystem_Abstraction SHALL use cross-platform compatible System.IO operations for all file and directory activities
4. THE Photo_Management_System SHALL target .NET 6 (net6.0) framework for cross-platform compatibility
5. THE Photo_Management_System SHALL include unit tests that validate cross-platform path handling and file operations

### Requirement 5

**User Story:** As a developer, I want the system to have a clean architecture with testable components, so that I can maintain and extend the functionality reliably.

#### Acceptance Criteria

1. THE FotoManager SHALL implement file system operations through the IFileSystem interface abstraction
2. THE Console_Application SHALL separate business logic in FotoManager from console I/O operations in Program.cs
3. THE Photo_Management_System SHALL provide dependency injection through constructor parameters and the Program.Manager property for testing
4. THE Photo_Management_System SHALL include comprehensive xUnit tests covering core functionality with MockFileSystem implementations
5. THE IFotoManger interface SHALL maintain backwards compatibility for public API changes while allowing internal method testing through InternalsVisibleTo

### Requirement 6

**User Story:** As a user, I want clear error handling and helpful messages, so that I can understand and resolve issues when they occur.

#### Acceptance Criteria

1. WHEN an invalid directory path is provided, THE Console_Application SHALL display the constant error message "the input path need double check !" with the attempted path
2. WHEN a baseline list file cannot be found or read, THE FotoManager SHALL return the constant error message "photo list file need double check !"
3. WHEN file operations fail during write operations, THE FotoManager SHALL handle exceptions gracefully and return appropriate error messages with file paths
4. THE Console_Application SHALL provide comprehensive help information when the --help parameter is used or when invalid parameters are provided
5. THE Console_Application SHALL validate all input parameters and display "Invalid parameter, switch to help mode should be: --help" for incorrect usage

### Requirement 7

**User Story:** As a user, I want the system to handle large photo collections efficiently, so that I can process thousands of files without performance issues.

#### Acceptance Criteria

1. THE File_Discovery_Engine SHALL process file collections using StringCollection data structures for efficient string operations
2. THE FotoManagerUtils SHALL sort filename collections alphabetically using Array.Sort for consistent and predictable output
3. THE Photo_Management_System SHALL process only file metadata (names, paths) without loading file contents into memory during scanning operations
4. THE Photo_Management_System SHALL display progress information including total file counts and operation completion status
5. THE Photo_Management_System SHALL handle file access scenarios through the FileSystem_Abstraction to allow proper error handling and testing
### Requ
irement 8

**User Story:** As a developer, I want the system to support future web service migration, so that the tool can evolve from CLI to web-based without major architectural changes.

#### Acceptance Criteria

1. THE FotoManager SHALL encapsulate all business logic independently from console I/O operations
2. THE IFotoManger interface SHALL expose all core operations (CreateListFile, GenerateDiffReports, CleanPhoto) as public methods
3. THE FileSystem_Abstraction SHALL provide complete file system operation abstraction to support different deployment environments
4. THE Photo_Management_System SHALL maintain clear separation between data processing logic and user interface concerns
5. THE Photo_Management_System SHALL use dependency injection patterns that support both console and web service hosting models