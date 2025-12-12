using DivinityModManager.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DivinityModManager.Services;

/// <summary>
/// Platform-agnostic service for loading and managing game mods.
/// Abstracts away LSLib dependency to allow cross-platform builds.
///
/// Windows implementation: Uses LSLib for full mod parsing from PAK/LSX files
/// Linux implementation: Returns empty/stub data with graceful degradation
/// </summary>
public interface IModLoadingService
{
	/// <summary>
	/// Indicates whether this platform supports full mod loading functionality.
	/// </summary>
	bool IsSupported { get; }

	/// <summary>
	/// Human-readable message explaining any limitations on this platform.
	/// </summary>
	string LimitationMessage { get; }

	/// <summary>
	/// Loads builtin game mods from the game installation directory.
	/// </summary>
	/// <param name="gameDataPath">Path to the game data directory</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>List of loaded builtin mods</returns>
	Task<List<DivinityModData>> LoadBuiltinModsAsync(
		string gameDataPath,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Loads editor project mods from the Mods folder in game data directory.
	/// </summary>
	/// <param name="gameDataPath">Path to the game data directory</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>List of loaded editor project mods</returns>
	Task<List<DivinityModData>> LoadEditorProjectsAsync(
		string gameDataPath,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Loads user mod packages (.pak files) from the mods directory.
	/// </summary>
	/// <param name="modsPath">Path to the mods directory containing .pak files</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Result object containing loaded mods and any errors</returns>
	Task<ModLoadingResults> LoadModPackageDataAsync(
		string modsPath,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Loads user profile data from saved profiles.
	/// </summary>
	/// <param name="profilesPath">Path to the profiles directory</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>List of loaded profiles</returns>
	Task<List<DivinityProfileData>> LoadProfilesAsync(
		string profilesPath,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the UUID of the currently selected/active profile.
	/// </summary>
	/// <param name="profilesPath">Path to the profiles directory</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>UUID of the selected profile, or null if none selected</returns>
	Task<string> GetSelectedProfileUUIDAsync(
		string profilesPath,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Exports mod settings (load order) to the game's modsettings.lsx file.
	/// </summary>
	/// <param name="filePath">Path to write the modsettings.lsx file</param>
	/// <param name="mods">List of mods in load order to export</param>
	/// <param name="cancellationToken">Cancellation token</param>
	Task ExportModSettingsAsync(
		string filePath,
		List<DivinityModData> mods,
		CancellationToken cancellationToken = default);
}
