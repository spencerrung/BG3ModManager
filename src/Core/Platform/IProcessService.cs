namespace DivinityModManager.Platform;

/// <summary>
/// Provides cross-platform process and application launching functionality.
/// Handles launching game processes, opening URLs, and opening file explorers.
/// </summary>
public interface IProcessService
{
    /// <summary>
    /// Launches a process (typically the game executable).
    /// </summary>
    /// <param name="processPath">Full path to the executable</param>
    /// <param name="arguments">Command-line arguments</param>
    /// <exception cref="FileNotFoundException">If the executable doesn't exist</exception>
    /// <exception cref="UnauthorizedAccessException">If access is denied</exception>
    void LaunchGame(string processPath, string arguments);

    /// <summary>
    /// Opens a URL in the default web browser.
    /// </summary>
    /// <param name="url">The URL to open</param>
    /// <exception cref="InvalidOperationException">If no suitable browser found</exception>
    void OpenUrl(string url);

    /// <summary>
    /// Opens a folder in the default file explorer.
    /// </summary>
    /// <param name="folderPath">The folder path to open</param>
    /// <exception cref="DirectoryNotFoundException">If the folder doesn't exist</exception>
    /// <exception cref="InvalidOperationException">If no suitable file explorer found</exception>
    void OpenFolder(string folderPath);
}
