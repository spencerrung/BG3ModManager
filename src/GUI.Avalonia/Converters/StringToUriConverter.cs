using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DivinityModManager.Converters;

public class StringToUriConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is string str)
		{
			if (Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out var result))
			{
				return result;
			}
		}
		return null;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is Uri uri)
		{
			return uri.OriginalString;
		}
		return "";
	}
}
