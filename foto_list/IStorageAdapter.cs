using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace foto_list
{
    /// <summary>
    /// Abstraction for storage operations that can be implemented by file system, database, or cloud storage
    /// </summary>
    public interface IStorageAdapter
    {
        /// <summary>
        /// Check if a location (directory/container) exists
        /// </summary>
        Task<bool> LocationExistsAsync(string location);
        
        /// <summary>
        /// Get the full path/identifier for a location
        /// </summary>
        Task<string> GetFullLocationAsync(string location);
        
        /// <summary>
        /// Get all items in a location matching the search pattern
        /// </summary>
        Task<IEnumerable<StorageItem>> GetItemsAsync(string location, string searchPattern);
        
        /// <summary>
        /// Get all sub-locations (subdirectories/containers) in a location
        /// </summary>
        Task<IEnumerable<string>> GetSubLocationsAsync(string location);
        
        /// <summary>
        /// Create a new location (directory/container)
        /// </summary>
        Task CreateLocationAsync(string location);
        
        /// <summary>
        /// Check if a specific item exists
        /// </summary>
        Task<bool> ItemExistsAsync(string itemPath);
        
        /// <summary>
        /// Move an item from source to target location
        /// </summary>
        Task MoveItemAsync(string sourcePath, string targetPath);
        
        /// <summary>
        /// Combine location paths in a storage-appropriate way
        /// </summary>
        string CombineLocation(params string[] paths);
        
        /// <summary>
        /// Get item name without extension
        /// </summary>
        string GetItemNameWithoutExtension(string itemPath);
        
        /// <summary>
        /// Get item name with extension
        /// </summary>
        string GetItemName(string itemPath);
        
        /// <summary>
        /// Get item extension
        /// </summary>
        string GetItemExtension(string itemPath);
    }
}