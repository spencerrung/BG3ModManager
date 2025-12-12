using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace DivinityModManager.Converters;

public class StringToLinearBrushConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is string str)
		{
			try
			{
				var startColor = Color.Parse(str);
				// Darken the color by 30%
				var endColor = Color.FromArgb(
					startColor.A,
					(byte)(startColor.R * 0.7),
					(byte)(startColor.G * 0.7),
					(byte)(startColor.B * 0.7)
				);
				return new LinearGradientBrush
				{
					StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
					EndPoint = new Avalonia.RelativePoint(0, 1, Avalonia.RelativeUnit.Relative),
					GradientStops = new GradientStops
					{
						new GradientStop(startColor, 0),
						new GradientStop(endColor, 1)
					}
				};
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
		if (value is Uri uri)
		{
			return uri.OriginalString;
		}
		return "";
	}
}
