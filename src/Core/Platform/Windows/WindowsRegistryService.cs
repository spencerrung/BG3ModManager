using Microsoft.Win32;
using System;

namespace DivinityModManager.Platform.Windows;

/// <summary>
/// Windows implementation of registry service using the Windows Registry API.
/// Handles reading and writing registry values for configuration storage.
/// </summary>
public class WindowsRegistryService : IRegistryService
{
	public string? GetValue(string path, string key)
	{
		try
		{
			var (hive, subPath) = ParseRegistryPath(path);

			using (var regKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
			using (var subKey = regKey.OpenSubKey(subPath, writable: false))
			{
				if (subKey == null)
				{
					DivinityApp.Log($"Registry subkey not found: {path}");
					return null;
				}

				var value = subKey.GetValue(key);
				return value?.ToString();
			}
		}
		catch (UnauthorizedAccessException ex)
		{
			DivinityApp.Log($"Unauthorized access to registry key {path}: {ex.Message}");
			throw;
		}
		catch (System.IO.IOException ex)
		{
			DivinityApp.Log($"Error reading registry key {path}: {ex.Message}");
			return null;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Unexpected error reading registry: {ex.Message}");
			throw;
		}
	}

	public void SetValue(string path, string key, string value)
	{
		try
		{
			var (hive, subPath) = ParseRegistryPath(path);

			using (var regKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
			{
				using (var subKey = regKey.CreateSubKey(subPath, writable: true))
				{
					if (subKey == null)
					{
						throw new InvalidOperationException($"Could not create or open registry subkey: {path}");
					}

					subKey.SetValue(key, value, RegistryValueKind.String);
				}
			}
		}
		catch (UnauthorizedAccessException ex)
		{
			DivinityApp.Log($"Unauthorized access to registry key {path}: {ex.Message}");
			throw;
		}
		catch (System.IO.IOException ex)
		{
			DivinityApp.Log($"Error writing registry key {path}: {ex.Message}");
			throw;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Unexpected error writing registry: {ex.Message}");
			throw;
		}
	}

	/// <summary>
	/// Parses a registry path string into hive and subpath components.
	/// Expected format: "HKEY_CURRENT_USER\Software\Company\App"
	/// </summary>
	private (RegistryHive, string) ParseRegistryPath(string path)
	{
		var parts = path.Split(new[] { '\\' }, 2);

		if (parts.Length < 1)
		{
			throw new ArgumentException($"Invalid registry path format: {path}", nameof(path));
		}

		var hiveString = parts[0];
		var subPath = parts.Length > 1 ? parts[1] : string.Empty;

		var hive = hiveString switch
		{
			"HKEY_CLASSES_ROOT" => RegistryHive.ClassesRoot,
			"HKEY_CURRENT_USER" => RegistryHive.CurrentUser,
			"HKEY_LOCAL_MACHINE" => RegistryHive.LocalMachine,
			"HKEY_USERS" => RegistryHive.Users,
			"HKEY_CURRENT_CONFIG" => RegistryHive.CurrentConfig,
			"HKEY_PERFORMANCE_DATA" => RegistryHive.PerformanceData,
			_ => throw new ArgumentException($"Unknown registry hive: {hiveString}", nameof(path))
		};

		return (hive, subPath);
	}
}
