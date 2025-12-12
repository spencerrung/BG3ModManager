namespace DivinityModManager.Platform;

/// <summary>
/// Aggregates all platform-specific services for cross-platform functionality.
/// Provides access to OS-specific features like file dialogs, registry, file system operations, etc.
/// </summary>
public interface IPlatformServices
{
    /// <summary>
    /// Service for opening file and folder dialogs.
    /// </summary>
    IFileDialogService FileDialogs { get; }

    /// <summary>
    /// Service for accessing system registry (Windows) or configuration files (Linux).
    /// </summary>
    IRegistryService Registry { get; }

    /// <summary>
    /// Service for file system operations like deletion to recycle bin, junction points, etc.
    /// </summary>
    IFileSystemService FileSystem { get; }

    /// <summary>
    /// Service for launching processes, opening URLs, and folders.
    /// </summary>
    IProcessService Process { get; }

    /// <summary>
    /// Service for accessibility features like screen reader announcements.
    /// </summary>
    IAccessibilityService Accessibility { get; }
}
