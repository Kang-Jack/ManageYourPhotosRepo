# Implementation Plan

- [x] 1. Create storage abstraction interfaces without breaking existing functionality



  - Create IStorageAdapter interface with async methods that mirror current IFileSystem operations
  - Create IListRepository interface for list persistence operations
  - Create StorageItem class and supporting data models
  - Ensure new interfaces can wrap existing IFileSystem implementation
  - _Requirements: 5.1, 8.1, 8.3_

- [ ] 2. Implement FileSystemAdapter as wrapper for current IFileSystem
  - [ ] 2.1 Create FileSystemAdapter class implementing IStorageAdapter
    - Wrap existing IFileSystem operations with async/await patterns
    - Convert file operations to StorageItem objects
    - Maintain exact same behavior as current FileSystem class
    - _Requirements: 4.1, 4.3, 5.1_

  - [ ] 2.2 Create FileListRepository class implementing IListRepository
    - Extract list reading/writing logic from FotoManager
    - Implement async file operations for list persistence
    - Maintain desktop file creation and naming conventions
    - _Requirements: 1.4, 2.4, 3.5_

  - [ ]* 2.3 Write unit tests for new adapter classes
    - Test FileSystemAdapter operations against mock IFileSystem
    - Test FileListRepository with temporary files
    - Verify async operation behavior and error handling
    - _Requirements: 5.4, 6.3_

- [ ] 3. Update FotoManager to use new abstractions while maintaining compatibility
  - [ ] 3.1 Add constructor overload accepting IStorageAdapter and IListRepository
    - Keep existing constructor with IFileSystem for backward compatibility
    - Create internal adapter instances when using legacy constructor
    - Ensure all existing tests continue to pass
    - _Requirements: 5.1, 5.5, 8.1_

  - [ ] 3.2 Refactor CreateListFile method to use new abstractions
    - Replace direct IFileSystem calls with IStorageAdapter operations
    - Use IListRepository for writing output files
    - Maintain exact same console output and file naming behavior
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

  - [ ] 3.3 Refactor GenerateDiffReports method to use new abstractions
    - Replace file operations with IStorageAdapter calls
    - Use IListRepository for reading baseline lists and writing reports
    - Preserve timestamp-based naming and desktop file creation
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

  - [ ] 3.4 Refactor CleanPhoto method to use new abstractions
    - Replace file operations with IStorageAdapter calls
    - Use IListRepository for reading baseline and writing reports
    - Maintain "removed" directory creation and file movement behavior
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [ ] 4. Add configuration support for future storage backend selection
  - [ ] 4.1 Create StorageConfiguration and related classes
    - Define StorageType enum and configuration classes
    - Create IStorageProviderFactory interface
    - Implement StorageProviderFactory with FileSystem provider
    - _Requirements: 8.2, 8.4_

  - [ ] 4.2 Update Program.cs to support dependency injection
    - Add optional configuration-based initialization
    - Maintain existing static Manager property for backward compatibility
    - Support both legacy direct instantiation and DI-based creation
    - _Requirements: 5.3, 8.5_

  - [ ]* 4.3 Add integration tests for configuration scenarios
    - Test factory creation of storage providers
    - Verify configuration-driven storage selection
    - Test backward compatibility with existing usage patterns
    - _Requirements: 5.4, 8.5_

- [ ] 5. Enhance error handling and cross-platform support
  - [ ] 5.1 Update error handling to work with new abstractions
    - Ensure async operations handle exceptions properly
    - Maintain existing error message constants and behavior
    - Add proper async exception handling patterns
    - _Requirements: 6.1, 6.2, 6.3_

  - [ ] 5.2 Verify cross-platform compatibility with new abstractions
    - Test path normalization through IStorageAdapter
    - Ensure async file operations work on Windows/Linux/macOS
    - Validate that existing cross-platform tests still pass
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_

  - [ ]* 5.3 Add performance tests for async operations
    - Compare performance of new async operations vs synchronous
    - Test memory usage with large file collections
    - Verify that async operations don't degrade performance
    - _Requirements: 7.1, 7.3, 7.5_

- [ ] 6. Create database adapter foundation for future migration
  - [ ] 6.1 Design database schema for photo and list storage
    - Create SQL scripts for Photos and PhotoLists tables
    - Design indexes for efficient querying by location and tenant
    - Include migration scripts from file-based to database storage
    - _Requirements: 8.1, 8.2_

  - [ ] 6.2 Implement DatabaseAdapter class (basic structure)
    - Create class implementing IStorageAdapter with database operations
    - Implement connection management and basic CRUD operations
    - Add configuration support for database connection strings
    - Mark as future implementation with proper interfaces
    - _Requirements: 8.1, 8.3_

  - [ ] 6.3 Implement DatabaseListRepository class (basic structure)
    - Create class implementing IListRepository with database operations
    - Support list storage and retrieval from database tables
    - Add proper async database operations with Entity Framework or ADO.NET
    - Mark as future implementation with proper interfaces
    - _Requirements: 8.1, 8.3_

- [ ] 7. Update documentation and prepare for web service migration
  - [ ] 7.1 Update XML documentation for new interfaces and classes
    - Document IStorageAdapter and IListRepository interfaces
    - Add code examples for different storage configurations
    - Update existing class documentation to reflect new patterns
    - _Requirements: 5.5, 8.4_

  - [ ] 7.2 Create migration guide for existing users
    - Document how existing functionality remains unchanged
    - Provide examples of new configuration options
    - Explain future database migration path
    - _Requirements: 8.5_

  - [ ]* 7.3 Add comprehensive integration tests
    - Test end-to-end workflows with new abstractions
    - Verify that all existing CLI scenarios work unchanged
    - Test configuration-driven storage selection
    - _Requirements: 5.4, 7.4, 8.5_