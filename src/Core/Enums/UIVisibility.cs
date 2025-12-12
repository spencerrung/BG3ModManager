namespace DivinityModManager.Enums;

/// <summary>
/// Cross-platform visibility enum replacing System.Windows.UIVisibility.
/// Used for UI property bindings that control element visibility.
/// </summary>
public enum UIVisibility
{
    /// <summary>
    /// Element is visible and takes up space in layout.
    /// </summary>
    Visible = 0,

    /// <summary>
    /// Element is hidden but still takes up space in layout.
    /// </summary>
    Hidden = 1,

    /// <summary>
    /// Element is hidden and does not take up space in layout.
    /// </summary>
    Collapsed = 2
}
