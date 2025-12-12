#if !WINDOWS && !(_WINDOWS)

using DivinityModManager.Models.Resources;

namespace DivinityModManager.Services.Linux;

/// <summary>
/// Linux stub implementation of IResourceService.
/// Resource parsing (PAK/LSF files) is not supported on Linux due to LSLib dependency.
/// This service provides graceful degradation and proper user messaging.
/// </summary>
public class LinuxResourceService : IResourceService
{
	private const string UnsupportedMessage =
		"Resource loading (mod PAK/LSF parsing) is not available on Linux. " +
		"This platform supports the GUI and configuration only. For full functionality, use Windows.";

	public bool IsSupported => false;
	public string LimitationMessage => UnsupportedMessage;

	public Task<IResourceData> LoadResourceAsync(string path, string format = "LSF")
	{
		DivinityApp.Log($"Cannot load resource on Linux: {path}");
		DivinityApp.Log(UnsupportedMessage);
		return Task.FromResult<IResourceData>(null);
	}

	public async Task SaveResourceAsync(IResourceData resource, string path, string format = "LSF")
	{
		await Task.Run(() =>
		{
			DivinityApp.Log($"Cannot save resource on Linux: {path}");
			DivinityApp.Log(UnsupportedMessage);
		});
	}

	public async Task<Dictionary<string, IResourceData>> ParseModMetadataAsync(string pakPath)
	{
		return await Task.Run(() =>
		{
			var results = new Dictionary<string, IResourceData>();

			if (!string.IsNullOrEmpty(pakPath))
			{
				DivinityApp.Log($"Cannot parse mod metadata on Linux: {pakPath}");
				DivinityApp.Log(UnsupportedMessage);
			}

			return results; // Return empty dictionary - no mods can be loaded on Linux
		});
	}

	public Task<IResourceData> ParseProfileDataAsync(string filePath)
	{
		if (!string.IsNullOrEmpty(filePath))
		{
			DivinityApp.Log($"Cannot parse profile data on Linux: {filePath}");
			DivinityApp.Log(UnsupportedMessage);
		}

		return Task.FromResult<IResourceData>(null); // Return null - profile data cannot be parsed
	}
}

#endif
