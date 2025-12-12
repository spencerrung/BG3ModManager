#if !WINDOWS && !(_WINDOWS)

using DivinityModManager.Models.Resources;

namespace DivinityModManager.Services.Linux;

/// <summary>
/// Linux stub implementation of IResourceData.
/// Returns empty structures - no actual resource parsing on Linux.
/// </summary>
internal class LinuxResourceDataStub : IResourceData
{
	private Dictionary<string, IRegionData> _regions = [];

	public LinuxResourceDataStub(uint version = 0)
	{
		Version = version;
	}

	public uint Version { get; set; }
	public IDictionary<string, IRegionData> Regions => _regions;

	public IRegionData GetRegion(string name)
	{
		_regions.TryGetValue(name, out var region);
		return region;
	}

	public bool TryGetRegion(string name, out IRegionData region)
	{
		return _regions.TryGetValue(name, out region);
	}
}

#endif
