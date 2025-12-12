#if WINDOWS || _WINDOWS

using DivinityModManager.Models;
using DivinityModManager.Util;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DivinityModManager.Services.Windows;

/// <summary>
/// Windows implementation of IModLoadingService.
/// Uses LSLib to load and parse mod data from PAK/LSX files.
/// </summary>
public class WindowsModLoadingService : IModLoadingService
{
	public bool IsSupported => true;
	public string LimitationMessage => "Mod loading fully supported on Windows";

	public async Task<List<DivinityModData>> LoadBuiltinModsAsync(
		string gameDataPath,
		CancellationToken cancellationToken = default)
	{
		return await DivinityModDataLoader.LoadBuiltinModsAsync(gameDataPath, cancellationToken);
	}

	public async Task<List<DivinityModData>> LoadEditorProjectsAsync(
		string gameDataPath,
		CancellationToken cancellationToken = default)
	{
		return await DivinityModDataLoader.LoadEditorProjectsAsync(gameDataPath, cancellationToken);
	}

	public async Task<ModLoadingResults> LoadModPackageDataAsync(
		string modsPath,
		CancellationToken cancellationToken = default)
	{
		return await DivinityModDataLoader.LoadModPackageDataAsync(modsPath, cancellationToken);
	}

	public async Task<List<DivinityProfileData>> LoadProfilesAsync(
		string profilesPath,
		CancellationToken cancellationToken = default)
	{
		return await DivinityModDataLoader.LoadProfileDataAsync(profilesPath, cancellationToken);
	}

	public async Task<string> GetSelectedProfileUUIDAsync(
		string profilesPath,
		CancellationToken cancellationToken = default)
	{
		return await DivinityModDataLoader.GetSelectedProfileUUIDAsync(profilesPath, cancellationToken);
	}

	public async Task ExportModSettingsAsync(
		string filePath,
		List<DivinityModData> mods,
		CancellationToken cancellationToken = default)
	{
		// Export mod settings by delegating to the loader
		// Note: ExportModSettingsToFileAsync takes a folder path and collection, not a file path
		await DivinityModDataLoader.ExportModSettingsToFileAsync(filePath, mods);
	}
}

#endif
