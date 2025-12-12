using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace DivinityModManager.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
	public static bool FromBool(bool b) => b;

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

		if (value is bool b)
		{
			if (!reverse)
			{
				return b;
			}
			else
			{
				return !b;
			}
		}
		return true;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is bool visible)
		{
			return visible;
		}
		return false;
	}
}

public class BoolToVisibilityConverterReversed : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is bool b)
		{
			return !b;
		}
		return false;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is bool visible)
		{
			return !visible;
		}
		return true;
	}
}
