#if !WINDOWS && !(_WINDOWS)

using DivinityModManager.Models.Resources;

namespace DivinityModManager.Services.Linux;

/// <summary>
/// Linux stub implementation of IRegionData.
/// Returns empty structures - no actual resource parsing on Linux.
/// </summary>
internal class LinuxRegionDataStub : IRegionData
{
	private Dictionary<string, List<INodeData>> _children = [];

	public LinuxRegionDataStub(string name = "", string regionName = "")
	{
		Name = name;
		RegionName = regionName;
	}

	public string Name { get; set; }
	public string RegionName { get; set; }
	public IDictionary<string, List<INodeData>> Children => _children;
}

#endif
