namespace DivinityModManager.Platform;

/// <summary>
/// Provides cross-platform accessibility features, primarily screen reader support.
/// </summary>
public interface IAccessibilityService
{
    /// <summary>
    /// Announces a message via screen readers (synchronously).
    /// For messages that should be announced immediately without blocking.
    /// </summary>
    /// <param name="message">The message to announce</param>
    void Announce(string message);

    /// <summary>
    /// Announces a message via screen readers (asynchronously).
    /// For longer operations or when blocking the UI thread is undesirable.
    /// </summary>
    /// <param name="message">The message to announce</param>
    /// <returns>A task that completes when the announcement is done</returns>
    Task AnnounceAsync(string message);
}
