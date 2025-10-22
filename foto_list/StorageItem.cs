using System;

namespace foto_list
{
    /// <summary>
    /// Represents an item in storage (file, blob, database record, etc.)
    /// </summary>
    public class StorageItem
    {
        /// <summary>
        /// The name of the item with extension
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The full path or identifier of the item in storage
        /// </summary>
        public string FullPath { get; set; }
        
        /// <summary>
        /// The name of the item without extension
        /// </summary>
        public string NameWithoutExtension { get; set; }
        
        /// <summary>
        /// When the item was last modified
        /// </summary>
        public DateTime LastModified { get; set; }
        
        /// <summary>
        /// Size of the item in bytes
        /// </summary>
        public long Size { get; set; }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public StorageItem()
        {
        }
        
        /// <summary>
        /// Constructor with basic properties
        /// </summary>
        public StorageItem(string name, string fullPath, string nameWithoutExtension)
        {
            Name = name;
            FullPath = fullPath;
            NameWithoutExtension = nameWithoutExtension;
        }
        
        /// <summary>
        /// Constructor with all properties
        /// </summary>
        public StorageItem(string name, string fullPath, string nameWithoutExtension, DateTime lastModified, long size)
        {
            Name = name;
            FullPath = fullPath;
            NameWithoutExtension = nameWithoutExtension;
            LastModified = lastModified;
            Size = size;
        }
    }
}