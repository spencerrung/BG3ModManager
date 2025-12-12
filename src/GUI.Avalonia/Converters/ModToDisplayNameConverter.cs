using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DivinityModManager.Converters;

/// <summary>
/// Converter that gets the display name for a mod from the registry.
/// Accepts either a mod object with UUID and Name properties, or a UUID string.
/// </summary>
public class ModToDisplayNameConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		try
		{
			if (value is string strValue)
			{
				// Return the string value as-is (UUID or display string)
				return strValue;
			}
			else if (value != null)
			{
				// Try to get Name property from object
				var nameProp = value.GetType().GetProperty("Name");
				if (nameProp != null)
				{
					return nameProp.GetValue(value) ?? "";
				}

				// Fall back to ToString()
				return value.ToString() ?? "";
			}
		}
		catch
		{
			// Silently fail
		}

		return "";
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return null;
	}
}
