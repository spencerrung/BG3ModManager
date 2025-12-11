using DivinityModManager.Enums;

namespace DivinityModManager.Util;

public static class PropertyConverters
{
	public static UIVisibility BoolToVisibility(bool b) => b ? UIVisibility.Visible : UIVisibility.Collapsed;
	public static UIVisibility BoolToVisibilityReversed(bool b) => !b ? UIVisibility.Visible : UIVisibility.Collapsed;
	public static UIVisibility BoolTupleToVisibility(ValueTuple<bool, bool, bool, bool, bool> b) => b.Item1 || b.Item2 || b.Item3 || b.Item4 || b.Item5 ? UIVisibility.Visible : UIVisibility.Collapsed;
	/// <summary>
	/// Visible if not null or empty, otherwise collapsed.
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	public static UIVisibility StringToVisibility(string str, UIVisibility fallback = UIVisibility.Collapsed) => !String.IsNullOrEmpty(str) ? UIVisibility.Visible : fallback;
	public static UIVisibility StringToVisibility(string str) => StringToVisibility(str, UIVisibility.Collapsed);
	public static UIVisibility StringToVisibilityReversed(string str, UIVisibility fallback = UIVisibility.Collapsed) => String.IsNullOrEmpty(str) ? UIVisibility.Visible : fallback;
	public static UIVisibility StringToVisibilityReversed(string str) => StringToVisibilityReversed(str, UIVisibility.Collapsed);
	public static UIVisibility UriToVisibility(Uri uri) => !String.IsNullOrEmpty(uri?.ToString()) ? UIVisibility.Visible : UIVisibility.Collapsed;
	public static UIVisibility IntToVisibility(int i) => i > 0 ? UIVisibility.Visible : UIVisibility.Collapsed;
}
