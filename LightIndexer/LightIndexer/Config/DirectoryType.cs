namespace LightIndexer.Config
{
    /// <summary>
    /// type of directory used by index
    /// </summary>
    internal enum DirectoryType
    {
        /// <summary>
        /// Disk - uses filesystem folder for stroing index
        /// </summary>
        DISK = 0,
        /// <summary>
        /// RAM - puts values only into memory, so index will not persist after proigram restart
        /// </summary>
        RAM = 1,
    }
}