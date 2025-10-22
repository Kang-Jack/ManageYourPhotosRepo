using System.Collections.Generic;
using System.Threading.Tasks;

namespace foto_list
{
    /// <summary>
    /// Abstraction for list persistence operations that can be implemented by file system, database, or other storage
    /// </summary>
    public interface IListRepository
    {
        /// <summary>
        /// Read a list of items from storage using the list identifier
        /// </summary>
        Task<IEnumerable<string>> ReadListAsync(string listIdentifier);
        
        /// <summary>
        /// Write a list of items to storage and return the actual storage location/identifier
        /// </summary>
        Task<string> WriteListAsync(string listIdentifier, IEnumerable<string> items);
        
        /// <summary>
        /// Check if a list exists in storage
        /// </summary>
        Task<bool> ListExistsAsync(string listIdentifier);
        
        /// <summary>
        /// Generate a unique list identifier based on base name and optional prefix
        /// Used for creating timestamped or unique list names
        /// </summary>
        Task<string> GenerateListIdentifierAsync(string baseName, string prefix = null);
    }
}