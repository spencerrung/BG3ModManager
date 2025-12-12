using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace DivinityModManager.Converters;

public class StringToSolidBrushConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is string str)
		{
			try
			{
				var color = Color.Parse(str);
				return new SolidColorBrush(color);
			}
			catch
			{
				// Silently fail
			}
		}
		return null;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return null;
	}
}
