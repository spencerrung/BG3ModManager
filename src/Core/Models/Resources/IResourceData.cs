namespace DivinityModManager.Models.Resources;

/// <summary>
/// Platform-agnostic interface for game resources.
/// Abstracts away LSLib.LS.Resource to allow cross-platform builds.
/// </summary>
public interface IResourceData
{
	uint Version { get; set; }
	IDictionary<string, IRegionData> Regions { get; }

	IRegionData GetRegion(string name);
	bool TryGetRegion(string name, out IRegionData region);
}
