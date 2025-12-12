#if !WINDOWS && !(_WINDOWS)

using DivinityModManager.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DivinityModManager.Services.Linux;

/// <summary>
/// Linux stub implementation of IModLoadingService.
/// Mod loading (PAK/LSF file parsing) is not supported on Linux due to LSLib dependency.
/// This service provides graceful degradation and proper user messaging.
/// </summary>
public class LinuxModLoadingService : IModLoadingService
{
	private const string UnsupportedMessage =
		"Mod loading from PAK/LSX files is not available on Linux. " +
		"This platform supports the GUI and configuration only. For full functionality, use Windows.";

	public bool IsSupported => false;
	public string LimitationMessage => UnsupportedMessage;

	public Task<List<DivinityModData>> LoadBuiltinModsAsync(
		string gameDataPath,
		CancellationToken cancellationToken = default)
	{
		DivinityApp.Log($"Cannot load builtin mods on Linux: {gameDataPath}");
		DivinityApp.Log(UnsupportedMessage);
		return Task.FromResult(new List<DivinityModData>());
	}

	public Task<List<DivinityModData>> LoadEditorProjectsAsync(
		string gameDataPath,
		CancellationToken cancellationToken = default)
	{
		DivinityApp.Log($"Cannot load editor projects on Linux: {gameDataPath}");
		DivinityApp.Log(UnsupportedMessage);
		return Task.FromResult(new List<DivinityModData>());
	}

	public Task<ModLoadingResults> LoadModPackageDataAsync(
		string modsPath,
		CancellationToken cancellationToken = default)
	{
		DivinityApp.Log($"Cannot load mod packages on Linux: {modsPath}");
		DivinityApp.Log(UnsupportedMessage);

		return Task.FromResult(new ModLoadingResults
		{
			Mods = new List<DivinityModData>(),
			Errors = new List<string> { UnsupportedMessage }
		});
	}

	public Task<List<DivinityProfileData>> LoadProfilesAsync(
		string profilesPath,
		CancellationToken cancellationToken = default)
	{
		DivinityApp.Log($"Cannot load profiles on Linux: {profilesPath}");
		DivinityApp.Log(UnsupportedMessage);
		return Task.FromResult(new List<DivinityProfileData>());
	}

	public Task<string> GetSelectedProfileUUIDAsync(
		string profilesPath,
		CancellationToken cancellationToken = default)
	{
		DivinityApp.Log($"Cannot get selected profile on Linux: {profilesPath}");
		return Task.FromResult<string>(null);
	}

	public Task ExportModSettingsAsync(
		string filePath,
		List<DivinityModData> mods,
		CancellationToken cancellationToken = default)
	{
		DivinityApp.Log($"Cannot export mod settings on Linux: {filePath}");
		DivinityApp.Log(UnsupportedMessage);
		return Task.CompletedTask;
	}
}

#endif
