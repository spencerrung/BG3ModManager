using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DivinityModManager.Converters;

/// <summary>
/// Generic enum to string converter.
/// </summary>
public class EnumToStringConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value?.ToString() ?? "";
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is string strValue && targetType?.IsEnum == true)
		{
			try
			{
				return Enum.Parse(targetType, strValue, true);
			}
			catch
			{
				return null;
			}
		}
		return null;
	}
}
