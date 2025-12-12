namespace DivinityModManager.Platform;

/// <summary>
/// Provides cross-platform file system operations.
/// Handles OS-specific features like recycle bin deletion, junction points (Windows) vs symlinks (Linux), etc.
/// </summary>
public interface IFileSystemService
{
    /// <summary>
    /// Deletes a file or directory, sending it to the recycle bin/trash instead of permanently deleting it.
    /// </summary>
    /// <param name="path">The path to delete</param>
    /// <exception cref="FileNotFoundException">If the path doesn't exist</exception>
    /// <exception cref="UnauthorizedAccessException">If access is denied</exception>
    void DeleteToRecycleBin(string path);

    /// <summary>
    /// Creates a junction point (Windows) or symbolic link (Linux) from a link path to a target path.
    /// </summary>
    /// <param name="linkPath">The path where the junction/symlink will be created</param>
    /// <param name="targetPath">The path the junction/symlink points to</param>
    /// <exception cref="DirectoryNotFoundException">If the target path doesn't exist</exception>
    /// <exception cref="UnauthorizedAccessException">If access is denied</exception>
    void CreateJunctionPoint(string linkPath, string targetPath);

    /// <summary>
    /// Deletes a junction point (Windows) or symbolic link (Linux).
    /// </summary>
    /// <param name="linkPath">The path to the junction/symlink to delete</param>
    /// <exception cref="FileNotFoundException">If the path doesn't exist</exception>
    /// <exception cref="UnauthorizedAccessException">If access is denied</exception>
    void DeleteJunctionPoint(string linkPath);

    /// <summary>
    /// Checks whether a path is a junction point (Windows) or symbolic link (Linux).
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>True if the path is a junction/symlink, false otherwise</returns>
    bool IsJunctionPoint(string path);
}
