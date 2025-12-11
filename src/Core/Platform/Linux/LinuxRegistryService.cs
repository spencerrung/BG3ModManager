using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DivinityModManager.Platform.Linux;

/// <summary>
/// Linux implementation of registry service using JSON-based configuration files.
/// Stores settings in XDG config directory (~/.config/bg3modmanager/)
/// </summary>
public class LinuxRegistryService : IRegistryService
{
	private readonly string _configDir;
	private readonly string _configFile;
	private Dictionary<string, Dictionary<string, string>> _config;

	public LinuxRegistryService()
	{
		_configDir = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
			".config", "bg3modmanager"
		);

		_configFile = Path.Combine(_configDir, "settings.json");

		try
		{
			// Create config directory if it doesn't exist
			if (!Directory.Exists(_configDir))
			{
				Directory.CreateDirectory(_configDir);
				DivinityApp.Log($"Created config directory: {_configDir}");
			}

			// Load existing config or create new one
			_config = LoadConfig();
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error initializing Linux registry service: {ex.Message}");
			_config = new Dictionary<string, Dictionary<string, string>>();
		}
	}

	public string? GetValue(string path, string key)
	{
		try
		{
			var section = ParsePath(path);

			if (_config.TryGetValue(section, out var sectionData))
			{
				if (sectionData.TryGetValue(key, out var value))
				{
					return value;
				}
			}

			return null;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error reading value from Linux registry: {ex.Message}");
			return null;
		}
	}

	public void SetValue(string path, string key, string value)
	{
		try
		{
			var section = ParsePath(path);

			// Ensure section exists
			if (!_config.ContainsKey(section))
			{
				_config[section] = new Dictionary<string, string>();
			}

			// Set the value
			_config[section][key] = value;

			// Save config to file
			SaveConfig();
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error writing value to Linux registry: {ex.Message}");
			throw;
		}
	}

	private string ParsePath(string path)
	{
		// Windows registry path: HKEY_CURRENT_USER\Software\Larian Studios\Baldur's Gate 3
		// We'll use the last component as the section name for simplicity
		// Example: "Baldur's Gate 3"

		var parts = path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

		if (parts.Length == 0)
		{
			throw new ArgumentException($"Invalid registry path: {path}", nameof(path));
		}

		// Use the last part of the path as the section name
		var section = parts[^1];

		// Sanitize section name for use as a JSON object key
		return SanitizeKey(section);
	}

	private string SanitizeKey(string key)
	{
		// Remove special characters that might cause JSON parsing issues
		return System.Text.RegularExpressions.Regex.Replace(key, @"[^\w\s-]", "_");
	}

	private Dictionary<string, Dictionary<string, string>> LoadConfig()
	{
		if (!File.Exists(_configFile))
		{
			return new Dictionary<string, Dictionary<string, string>>();
		}

		try
		{
			var json = File.ReadAllText(_configFile);
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				WriteIndented = true
			};

			var config = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json, options);
			return config ?? new Dictionary<string, Dictionary<string, string>>();
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error loading config file: {ex.Message}");
			return new Dictionary<string, Dictionary<string, string>>();
		}
	}

	private void SaveConfig()
	{
		try
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				WriteIndented = true,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
			};

			var json = JsonSerializer.Serialize(_config, options);
			File.WriteAllText(_configFile, json);

			// Set file permissions to 0644 (owner read/write, others read)
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				try
				{
					var fileInfo = new FileInfo(_configFile);
					// This is a best-effort operation; it may not work on all systems
					System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
					{
						FileName = "chmod",
						Arguments = "0644 " + _configFile,
						CreateNoWindow = true,
						UseShellExecute = false
					});
				}
				catch
				{
					// Silently ignore chmod errors
				}
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error saving config file: {ex.Message}");
			throw;
		}
	}
}
