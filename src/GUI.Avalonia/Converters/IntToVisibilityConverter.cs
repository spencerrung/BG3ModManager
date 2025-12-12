using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DivinityModManager.Converters;

public class IntToVisibilityConverter : IValueConverter
{
	public static bool FromInt(int v) => v > 0;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is int intVal)
		{
			return intVal != 0;
		}
		return true;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return null;
	}
}
