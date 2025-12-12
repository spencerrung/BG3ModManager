using DivinityModManager.Models.Input;

namespace DivinityModManager;

public static class KeyExtensions
{
	private static readonly Dictionary<KeyCode, string> KeyToName = new()
	{
		{KeyCode.D0, "0"},
		{KeyCode.D1, "1"},
		{KeyCode.D2, "2"},
		{KeyCode.D3, "3"},
		{KeyCode.D4, "4"},
		{KeyCode.D5, "5"},
		{KeyCode.D6, "6"},
		{KeyCode.D7, "7"},
		{KeyCode.D8, "8"},
		{KeyCode.D9, "9"},
		{KeyCode.Semicolon, ";"},
		{KeyCode.Backslash, "\\"},
		{KeyCode.RightBracket, "]"},
		{KeyCode.Quote, "'"},
		{KeyCode.Comma, ","},
		{KeyCode.Minus, "-"},
		{KeyCode.LeftBracket, "["},
		{KeyCode.Period, "."},
		{KeyCode.Equals, "="},
		{KeyCode.Slash, "/"},
		{KeyCode.Grave, "`"}
	};

	public static string GetKeyName(this KeyCode key)
	{
		if (KeyToName.TryGetValue(key, out string name))
		{
			return name;
		}
		return key.ToString();
	}
}
