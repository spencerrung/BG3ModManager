namespace DivinityModManager.Models.Input;

/// <summary>
/// Cross-platform key code enum replacing System.Windows.Input.Key.
/// Maps keyboard keys to cross-platform identifiers.
/// </summary>
public enum KeyCode
{
    None = 0,

    // Function Keys
    F1 = 112,
    F2 = 113,
    F3 = 114,
    F4 = 115,
    F5 = 116,
    F6 = 117,
    F7 = 118,
    F8 = 119,
    F9 = 120,
    F10 = 121,
    F11 = 122,
    F12 = 123,

    // Control Keys
    Escape = 27,
    Tab = 9,
    CapsLock = 20,
    LeftShift = 160,
    RightShift = 161,
    LeftCtrl = 162,
    RightCtrl = 163,
    LeftAlt = 164,
    RightAlt = 165,
    Enter = 13,
    Space = 32,
    Backspace = 8,
    Delete = 46,
    Insert = 45,
    Home = 36,
    End = 35,
    PageUp = 33,
    PageDown = 34,

    // Arrow Keys
    Up = 38,
    Down = 40,
    Left = 37,
    Right = 39,

    // Number Keys
    D0 = 48,
    D1 = 49,
    D2 = 50,
    D3 = 51,
    D4 = 52,
    D5 = 53,
    D6 = 54,
    D7 = 55,
    D8 = 56,
    D9 = 57,

    // Letter Keys
    A = 65,
    B = 66,
    C = 67,
    D = 68,
    E = 69,
    F = 70,
    G = 71,
    H = 72,
    I = 73,
    J = 74,
    K = 75,
    L = 76,
    M = 77,
    N = 78,
    O = 79,
    P = 80,
    Q = 81,
    R = 82,
    S = 83,
    T = 84,
    U = 85,
    V = 86,
    W = 87,
    X = 88,
    Y = 89,
    Z = 90,

    // Special Keys
    Semicolon = 186,
    Equals = 187,
    Comma = 188,
    Minus = 189,
    Period = 190,
    Slash = 191,
    Grave = 192,
    LeftBracket = 219,
    Backslash = 220,
    RightBracket = 221,
    Quote = 222,
}

/// <summary>
/// Modifier key flags.
/// </summary>
[Flags]
public enum ModifierKeys
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Windows = 8
}
