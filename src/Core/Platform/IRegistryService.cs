namespace DivinityModManager.Platform;

/// <summary>
/// Provides cross-platform registry/configuration access.
/// Windows: Accesses the system registry
/// Linux: Uses file-based configuration (e.g., ~/.config/bg3modmanager/settings.ini)
/// </summary>
public interface IRegistryService
{
    /// <summary>
    /// Gets a value from the registry/configuration.
    /// </summary>
    /// <param name="path">The registry path or config file path (e.g., "HKEY_CURRENT_USER\\Software\\Larian Studios\\Baldur's Gate 3")</param>
    /// <param name="key">The value name/key (e.g., "InstallLocation")</param>
    /// <returns>The value, or null if not found</returns>
    string? GetValue(string path, string key);

    /// <summary>
    /// Sets a value in the registry/configuration.
    /// </summary>
    /// <param name="path">The registry path or config file path</param>
    /// <param name="key">The value name/key</param>
    /// <param name="value">The value to set</param>
    /// <exception cref="UnauthorizedAccessException">If access is denied</exception>
    void SetValue(string path, string key, string value);
}
