#if !WINDOWS && !(_WINDOWS)

using DivinityModManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DivinityModManager.Services.Linux;

/// <summary>
/// Linux implementation of IModLoadingService.
/// Uses cross-platform PAK and LSX parsers to load mods without LSLib dependency.
/// Provides basic mod loading functionality for Linux users.
/// </summary>
public class LinuxModLoadingService : IModLoadingService
{
	private const string PartialModPattern = "_[0-9]+\\.pak$";

	public bool IsSupported => true;
	public string LimitationMessage => "Mod and profile loading is supported. Builtin mods (game data) and save game analysis are not yet available.";

	public Task<List<DivinityModData>> LoadBuiltinModsAsync(
		string gameDataPath,
		CancellationToken cancellationToken = default)
	{
		DivinityApp.Log($"Builtin mod loading not yet implemented on Linux: {gameDataPath}");
		DivinityApp.Log("This feature requires VFS scanning which is not yet ported from LSLib.");
		return Task.FromResult(new List<DivinityModData>());
	}

	public Task<List<DivinityModData>> LoadEditorProjectsAsync(
		string gameDataPath,
		CancellationToken cancellationToken = default)
	{
		DivinityApp.Log($"Editor project loading not yet implemented on Linux: {gameDataPath}");
		return Task.FromResult(new List<DivinityModData>());
	}

	public async Task<ModLoadingResults> LoadModPackageDataAsync(
		string modsPath,
		CancellationToken cancellationToken = default)
	{
		var results = new ModLoadingResults
		{
			Mods = new List<DivinityModData>(),
			Errors = new List<string>()
		};

		if (!Directory.Exists(modsPath))
		{
			DivinityApp.Log($"Mods directory not found: {modsPath}");
			results.Errors.Add($"Mods directory not found: {modsPath}");
			return results;
		}

		try
		{
			DivinityApp.Log($"Loading mods from: {modsPath}");

			// Find all PAK files in the mods directory (recursively)
			var pakFiles = Directory.GetFiles(modsPath, "*.pak", SearchOption.AllDirectories)
				.Where(f => !IsPartialPakFile(f))
				.ToList();

			DivinityApp.Log($"Found {pakFiles.Count} PAK files to load");

			// Load mods in parallel using ConcurrentBag for thread-safe collection
			var modBag = new ConcurrentBag<DivinityModData>();
			var loadTasks = pakFiles.Select(pakPath =>
				LoadModFromPakSafeAsync(pakPath, modBag, cancellationToken)
			).ToList();

			await Task.WhenAll(loadTasks);

			results.Mods = modBag.ToList();
			DivinityApp.Log($"Successfully loaded {results.Mods.Count} mods");

			return results;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error loading mod packages: {ex.Message}");
			results.Errors.Add($"Error loading mod packages: {ex.Message}");
			return results;
		}
	}

	public async Task<List<DivinityProfileData>> LoadProfilesAsync(
		string profilesPath,
		CancellationToken cancellationToken = default)
	{
		var profiles = new List<DivinityProfileData>();

		if (!Directory.Exists(profilesPath))
		{
			DivinityApp.Log($"Profiles directory not found: {profilesPath}");
			return profiles;
		}

		try
		{
			DivinityApp.Log($"Loading profiles from: {profilesPath}");

			// Enumerate all profile directories
			var profileDirectories = Directory.EnumerateDirectories(profilesPath);

			foreach (var profileFolder in profileDirectories)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				try
				{
					var profileData = await LoadProfileAsync(profileFolder, cancellationToken);
					if (profileData != null)
					{
						profiles.Add(profileData);
						DivinityApp.Log($"Loaded profile: {profileData.Name}");
					}
				}
				catch (Exception ex)
				{
					DivinityApp.Log($"Error loading profile from '{profileFolder}': {ex.Message}");
				}
			}

			DivinityApp.Log($"Successfully loaded {profiles.Count} profiles");
			return profiles;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error loading profiles: {ex.Message}");
			return profiles;
		}
	}

	public async Task<string> GetSelectedProfileUUIDAsync(
		string profilesPath,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var playerprofilesFile = FindPlayerProfilesFile(profilesPath);
			if (playerprofilesFile == null)
			{
				DivinityApp.Log($"No playerprofiles file found in: {profilesPath}");
				return null;
			}

			DivinityApp.Log($"Loading playerprofiles from: {playerprofilesFile}");

			var lsxParser = ServiceLocator.LsxParser;
			var fileContent = File.ReadAllText(playerprofilesFile);
			var xDoc = lsxParser.ParseXml(fileContent);

			// Find the ActiveProfile attribute in the UserProfiles region
			var regionNode = xDoc.Element("region");
			if (regionNode != null)
			{
				var activeProfileAttr = regionNode.Descendants("attribute")
					.FirstOrDefault(a => a.Attribute("id")?.Value == "ActiveProfile");

				if (activeProfileAttr != null)
				{
					var activeProfileUUID = activeProfileAttr.Attribute("value")?.Value;
					if (!string.IsNullOrEmpty(activeProfileUUID))
					{
						DivinityApp.Log($"Found active profile UUID: {activeProfileUUID}");
						return activeProfileUUID;
					}
				}
			}

			DivinityApp.Log("No active profile UUID found");
			return null;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error getting selected profile: {ex.Message}");
			return null;
		}
	}

	public Task ExportModSettingsAsync(
		string filePath,
		List<DivinityModData> mods,
		CancellationToken cancellationToken = default)
	{
		DivinityApp.Log($"Mod settings export not yet implemented on Linux: {filePath}");
		return Task.CompletedTask;
	}

	/// <summary>
	/// Checks if a PAK file is a partial/split PAK (e.g., mod_1.pak, mod_2.pak, etc.)
	/// </summary>
	private static bool IsPartialPakFile(string filePath)
	{
		var fileName = Path.GetFileName(filePath);
		return System.Text.RegularExpressions.Regex.IsMatch(fileName, PartialModPattern);
	}

	/// <summary>
	/// Safely loads a mod from a PAK file and adds it to the collection.
	/// </summary>
	private static async Task LoadModFromPakSafeAsync(
		string pakPath,
		ConcurrentBag<DivinityModData> modBag,
		CancellationToken cancellationToken)
	{
		try
		{
			var modData = await LoadModFromPakAsync(pakPath, cancellationToken);
			if (modData != null)
			{
				modBag.Add(modData);
				DivinityApp.Log($"Loaded mod: {modData.Name} ({modData.UUID})");
			}
			else
			{
				DivinityApp.Log($"Warning: Could not parse mod from {pakPath}");
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error loading mod from '{pakPath}': {ex.Message}");
		}
	}

	/// <summary>
	/// Loads mod metadata from a PAK file using the cross-platform parsers.
	/// </summary>
	private static async Task<DivinityModData> LoadModFromPakAsync(
		string pakPath,
		CancellationToken cancellationToken)
	{
		if (!File.Exists(pakPath))
		{
			DivinityApp.Log($"PAK file not found: {pakPath}");
			return null;
		}

		try
		{
			var pakReader = ServiceLocator.PakReader;
			pakReader.Open(pakPath);

			// Find meta.lsx file in the PAK
			var fileList = pakReader.GetFileList();
			var metaFile = fileList.FirstOrDefault(f =>
				Path.GetFileName(f.Name).Equals("meta.lsx", StringComparison.OrdinalIgnoreCase) &&
				f.Name.Contains("Mods/", StringComparison.OrdinalIgnoreCase)
			);

			if (metaFile == null)
			{
				DivinityApp.Log($"No meta.lsx found in PAK: {pakPath}");
				pakReader.Dispose();
				return null;
			}

			// Read and parse meta.lsx
			var metaContent = pakReader.ReadFileAsText(metaFile);
			var modData = ParseModMetadata(metaContent);

			if (modData != null)
			{
				modData.FilePath = pakPath;
				modData.IsUserMod = true;
				modData.Files = fileList.Select(f => f.Name).ToList();
			}

			pakReader.Dispose();
			return modData;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error loading mod from PAK '{pakPath}': {ex.Message}");
			return null;
		}
	}

	/// <summary>
	/// Parses mod metadata from LSX content.
	/// </summary>
	private static DivinityModData ParseModMetadata(string lsxContent)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(lsxContent))
			{
				return null;
			}

			var lsxParser = ServiceLocator.LsxParser;
			var xDoc = lsxParser.ParseXml(lsxContent);

			var modData = new DivinityModData();

			// Find ModuleInfo node
			var moduleInfoNode = xDoc.Descendants("node")
				.FirstOrDefault(n => n.Attribute("id")?.Value == "ModuleInfo");

			if (moduleInfoNode == null)
			{
				DivinityApp.Log($"No ModuleInfo found in meta.lsx");
				return null;
			}

			// Extract basic mod information
			modData.UUID = lsxParser.GetAttributeValueWithId(moduleInfoNode, "UUID", "");
			modData.Name = UnescapeXml(lsxParser.GetAttributeValueWithId(moduleInfoNode, "Name", ""));
			modData.Description = UnescapeXml(lsxParser.GetAttributeValueWithId(moduleInfoNode, "Description", ""));
			modData.Author = UnescapeXml(lsxParser.GetAttributeValueWithId(moduleInfoNode, "Author", ""));
			modData.Folder = lsxParser.GetAttributeValueWithId(moduleInfoNode, "Folder", "");
			modData.MD5 = lsxParser.GetAttributeValueWithId(moduleInfoNode, "MD5", "");
			modData.ModType = lsxParser.GetAttributeValueWithId(moduleInfoNode, "Type", "");

			modData.HasMetadata = true;
			return modData;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error parsing mod metadata: {ex.Message}");
			return null;
		}
	}

	/// <summary>
	/// Unescapes XML entities.
	/// </summary>
	private static string UnescapeXml(string text)
	{
		if (string.IsNullOrEmpty(text))
			return text;

		return text
			.Replace("&lt;", "<")
			.Replace("&gt;", ">")
			.Replace("&amp;", "&")
			.Replace("&quot;", "\"")
			.Replace("&apos;", "'");
	}

	/// <summary>
	/// Loads a single profile from its directory.
	/// </summary>
	private static async Task<DivinityProfileData> LoadProfileAsync(
		string profileFolder,
		CancellationToken cancellationToken)
	{
		if (!Directory.Exists(profileFolder))
		{
			return null;
		}

		var folderName = Path.GetFileName(profileFolder);
		var modSettingsFile = Path.Combine(profileFolder, "modsettings.lsx");

		// Create basic profile data
		var profileData = new DivinityProfileData(folderName, modSettingsFile)
		{
			Name = folderName,
			ProfileName = folderName
		};

		// Load active mods from modsettings.lsx
		var activeMods = await LoadActiveModsFromModSettingsAsync(modSettingsFile, cancellationToken);
		profileData.ActiveMods = activeMods;

		return profileData;
	}

	/// <summary>
	/// Loads the active mods list from a modsettings.lsx file.
	/// </summary>
	private static async Task<List<DivinityProfileActiveModData>> LoadActiveModsFromModSettingsAsync(
		string modSettingsPath,
		CancellationToken cancellationToken)
	{
		var activeMods = new List<DivinityProfileActiveModData>();

		if (!File.Exists(modSettingsPath))
		{
			return activeMods;
		}

		try
		{
			var lsxParser = ServiceLocator.LsxParser;
			var fileContent = File.ReadAllText(modSettingsPath);
			var xDoc = lsxParser.ParseXml(fileContent);

			// Find the Mods node
			var modsNode = xDoc.Descendants("node")
				.FirstOrDefault(n => n.Attribute("id")?.Value == "Mods");

			if (modsNode == null)
			{
				return activeMods;
			}

			// Find all ModuleShortDesc nodes (individual mod entries)
			var modNodes = modsNode.Descendants("node")
				.Where(n => n.Attribute("id")?.Value == "ModuleShortDesc");

			foreach (var modNode in modNodes)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				var activeMod = new DivinityProfileActiveModData();

				// Extract mod attributes
				activeMod.Folder = lsxParser.GetAttributeValueWithId(modNode, "Folder", "");
				activeMod.MD5 = lsxParser.GetAttributeValueWithId(modNode, "MD5", "");
				activeMod.Name = UnescapeXml(lsxParser.GetAttributeValueWithId(modNode, "Name", ""));
				activeMod.UUID = lsxParser.GetAttributeValueWithId(modNode, "UUID", "");

				// Try to parse version
				var versionStr = lsxParser.GetAttributeValueWithId(modNode, "Version64", "");
				if (string.IsNullOrEmpty(versionStr))
				{
					versionStr = lsxParser.GetAttributeValueWithId(modNode, "Version", "");
				}

				if (ulong.TryParse(versionStr, out var version))
				{
					activeMod.Version = version;
				}

				// Only add non-ignored mods
				if (!string.IsNullOrEmpty(activeMod.UUID))
				{
					activeMods.Add(activeMod);
				}
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error loading mod settings from '{modSettingsPath}': {ex.Message}");
		}

		return activeMods;
	}

	/// <summary>
	/// Finds the playerprofiles file in the profiles directory.
	/// </summary>
	private static string FindPlayerProfilesFile(string profilesPath)
	{
		if (!Directory.Exists(profilesPath))
		{
			return null;
		}

		// Look for playerprofiles.lsx, playerprofiles.lsb, or playerprofiles.lsf
		var playerprofilesPatterns = new[] { "playerprofiles.lsx", "playerprofiles.lsb", "playerprofiles.lsf" };

		foreach (var pattern in playerprofilesPatterns)
		{
			var filePath = Path.Combine(profilesPath, pattern);
			if (File.Exists(filePath))
			{
				return filePath;
			}
		}

		return null;
	}

	/// <summary>
	/// Finds a profile file in a profile directory (profile.lsx, profile.lsb, etc.)
	/// </summary>
	private static string FindProfileFile(string profileFolder)
	{
		if (!Directory.Exists(profileFolder))
		{
			return null;
		}

		// Look for profile*.lsx, profile*.lsb files in order of preference
		var profilePatterns = new[] { "profile.lsx", "profile.lsb", "profile5.lsb", "profile.lsf" };

		foreach (var pattern in profilePatterns)
		{
			var filePath = Path.Combine(profileFolder, pattern);
			if (File.Exists(filePath))
			{
				return filePath;
			}
		}

		// If no exact match, try to find any file starting with "profile"
		var files = Directory.GetFiles(profileFolder, "profile*");
		return files.FirstOrDefault();
	}
}

#endif
