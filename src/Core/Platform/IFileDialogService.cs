namespace DivinityModManager.Platform;

/// <summary>
/// Provides cross-platform file dialog functionality.
/// Abstracts away platform-specific dialog implementations (Windows native dialogs, GTK dialogs, etc.).
/// </summary>
public interface IFileDialogService
{
    /// <summary>
    /// Opens a file selection dialog.
    /// </summary>
    /// <param name="title">The dialog title</param>
    /// <param name="filter">File filter (e.g., "Text Files|*.txt|All Files|*.*")</param>
    /// <returns>The selected file path, or null if user cancelled</returns>
    Task<string?> OpenFileAsync(string title, string filter);

    /// <summary>
    /// Opens a file selection dialog allowing multiple file selection.
    /// </summary>
    /// <param name="title">The dialog title</param>
    /// <param name="filter">File filter (e.g., "Text Files|*.txt|All Files|*.*")</param>
    /// <returns>List of selected file paths, or empty list if user cancelled</returns>
    Task<IReadOnlyList<string>> OpenFilesAsync(string title, string filter);

    /// <summary>
    /// Opens a folder selection dialog.
    /// </summary>
    /// <param name="title">The dialog title</param>
    /// <returns>The selected folder path, or null if user cancelled</returns>
    Task<string?> OpenFolderAsync(string title);

    /// <summary>
    /// Opens a save file dialog.
    /// </summary>
    /// <param name="title">The dialog title</param>
    /// <param name="filter">File filter (e.g., "Text Files|*.txt|All Files|*.*")</param>
    /// <param name="suggestedFileName">Default filename to suggest</param>
    /// <returns>The selected file path, or null if user cancelled</returns>
    Task<string?> SaveFileAsync(string title, string filter, string suggestedFileName);
}
