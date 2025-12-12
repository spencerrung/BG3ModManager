using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace DivinityModManager.Converters;

internal class UriToBitmapImageConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is Uri uri)
		{
			try
			{
				// Convert Uri to string path for Bitmap constructor
				return new Bitmap(uri.OriginalString);
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
		return "";
	}
}
