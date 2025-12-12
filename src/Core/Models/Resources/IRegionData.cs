namespace DivinityModManager.Models.Resources;

/// <summary>
/// Platform-agnostic interface for resource regions.
/// Abstracts away LSLib.LS.Region to allow cross-platform builds.
/// </summary>
public interface IRegionData
{
	string Name { get; set; }
	string RegionName { get; }
	IDictionary<string, List<INodeData>> Children { get; }
}
