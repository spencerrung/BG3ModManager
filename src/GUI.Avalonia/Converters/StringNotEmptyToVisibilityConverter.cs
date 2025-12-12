using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DivinityModManager.Converters;

public class StringNotEmptyToVisibilityConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		bool reverse = false;
		if (parameter != null)
		{
			if (parameter is int reverseInt)
			{
				reverse = reverseInt > 0;
			}
			else if (parameter is bool r)
			{
				reverse = r;
			}
		}

		if (value is string v)
		{
			if (!reverse)
			{
				return !string.IsNullOrWhiteSpace(v);
			}
			else
			{
				return string.IsNullOrWhiteSpace(v);
			}
		}
		return true;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return null;
	}
}
