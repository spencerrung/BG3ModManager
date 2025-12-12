#if WINDOWS || _WINDOWS

using DivinityModManager.Models.Resources;
using DivinityModManager.Util;
using LSLib.LS;

namespace DivinityModManager.Services.Windows;

/// <summary>
/// Windows implementation of IResourceService using LSLib for full resource parsing.
/// </summary>
public class WindowsResourceService : IResourceService
{
	public bool IsSupported => true;
	public string LimitationMessage => "";

	public async Task<IResourceData> LoadResourceAsync(string path, string format = "LSF")
	{
		return await Task.Run(() =>
		{
			try
			{
				if (!File.Exists(path))
				{
					DivinityApp.Log($"Resource file not found: {path}");
					return null;
				}

				var settings = new ResourceLoadParameters();
				var resource = ResourceUtils.LoadResource(path, settings);

				if (resource == null)
				{
					DivinityApp.Log($"Failed to load resource: {path}");
					return null;
				}

				return new WindowsResourceDataAdapter(resource);
			}
			catch (Exception ex)
			{
				DivinityApp.Log($"Error loading resource '{path}': {ex.Message}");
				return null;
			}
		});
	}

	public async Task SaveResourceAsync(IResourceData resource, string path, string format = "LSF")
	{
		return await Task.Run(() =>
		{
			try
			{
				if (resource is not WindowsResourceDataAdapter adapter)
				{
					throw new ArgumentException("Resource must be a Windows adapter", nameof(resource));
				}

				var parameters = ResourceConversionParameters.FromGameVersion(DivinityApp.GAME);
				var resourceFormat = format.Equals("LSX", StringComparison.OrdinalIgnoreCase)
					? LSLib.LS.Enums.ResourceFormat.LSX
					: LSLib.LS.Enums.ResourceFormat.LSF;

				ResourceUtils.SaveResource(adapter.UnderlyingResource, path, resourceFormat, parameters);
				DivinityApp.Log($"Saved resource to: {path}");
			}
			catch (Exception ex)
			{
				DivinityApp.Log($"Error saving resource to '{path}': {ex.Message}");
				throw;
			}
		});
	}

	public async Task<Dictionary<string, IResourceData>> ParseModMetadataAsync(string pakPath)
	{
		return await Task.Run(() =>
		{
			var results = new Dictionary<string, IResourceData>();

			try
			{
				if (!File.Exists(pakPath))
				{
					DivinityApp.Log($"PAK file not found: {pakPath}");
					return results;
				}

				// Use DivinityModDataLoader's existing logic to load PAK metadata
				// This maintains compatibility with the existing implementation
				var metadata = DivinityModDataLoader.LoadMetadataFromPak(pakPath);
				if (metadata != null)
				{
					foreach (var kvp in metadata)
					{
						results[kvp.Key] = new WindowsResourceDataAdapter(kvp.Value);
					}
				}

				return results;
			}
			catch (Exception ex)
			{
				DivinityApp.Log($"Error parsing mod metadata from '{pakPath}': {ex.Message}");
				return results;
			}
		});
	}

	public async Task<IResourceData> ParseProfileDataAsync(string filePath)
	{
		return await Task.Run(() =>
		{
			try
			{
				if (!File.Exists(filePath))
				{
					DivinityApp.Log($"Profile file not found: {filePath}");
					return null;
				}

				// Use existing DivinityModDataLoader logic for profile parsing
				var resource = DivinityModDataLoader.LoadProfileResource(filePath);
				if (resource == null)
				{
					DivinityApp.Log($"Failed to load profile resource: {filePath}");
					return null;
				}

				return new WindowsResourceDataAdapter(resource);
			}
			catch (Exception ex)
			{
				DivinityApp.Log($"Error parsing profile data from '{filePath}': {ex.Message}");
				return null;
			}
		});
	}
}

#endif
