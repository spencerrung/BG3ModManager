using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DivinityModManager.Converters;

/// <summary>
/// Converter that checks if a mod UUID exists in the mod registry.
/// Note: Placeholder converter - actual registry checking requires service initialization.
/// </summary>
public class ModExistsConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		// Placeholder implementation - returns false for now
		// This converter would need IModRegistryService to function
		return false;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return null;
	}
}
