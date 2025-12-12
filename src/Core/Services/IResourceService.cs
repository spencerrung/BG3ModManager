using DivinityModManager.Models.Resources;

namespace DivinityModManager.Services;

/// <summary>
/// Platform-agnostic service for loading and managing game resources.
/// Abstracts away LSLib dependency to allow cross-platform builds.
///
/// Windows implementation: Uses LSLib for full PAK/LSF parsing
/// Linux implementation: Returns stub data with graceful degradation
/// </summary>
public interface IResourceService
{
	/// <summary>
	/// Loads a resource file from the given path.
	/// </summary>
	Task<IResourceData> LoadResourceAsync(string path, string format = "LSF");

	/// <summary>
	/// Saves a resource to the given path.
	/// </summary>
	Task SaveResourceAsync(IResourceData resource, string path, string format = "LSF");

	/// <summary>
	/// Parses mod metadata from a PAK file.
	/// Returns null if parsing fails or platform doesn't support it.
	/// </summary>
	Task<Dictionary<string, IResourceData>> ParseModMetadataAsync(string pakPath);

	/// <summary>
	/// Parses profile data from a file.
	/// Returns null if parsing fails or platform doesn't support it.
	/// </summary>
	Task<IResourceData> ParseProfileDataAsync(string filePath);

	/// <summary>
	/// Gets whether this platform supports resource parsing.
	/// </summary>
	bool IsSupported { get; }

	/// <summary>
	/// Gets a user-friendly message explaining why resource loading might be unavailable.
	/// Empty string if fully supported.
	/// </summary>
	string LimitationMessage { get; }
}
