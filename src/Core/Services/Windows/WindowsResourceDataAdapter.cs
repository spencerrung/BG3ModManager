#if WINDOWS || _WINDOWS

using DivinityModManager.Models.Resources;
using LSLib.LS;

namespace DivinityModManager.Services.Windows;

/// <summary>
/// Adapts LSLib.LS.Resource to IResourceData interface for Windows.
/// </summary>
internal class WindowsResourceDataAdapter : IResourceData
{
	private readonly Resource _resource;
	private Dictionary<string, IRegionData> _regionsCache;

	public WindowsResourceDataAdapter(Resource resource)
	{
		_resource = resource ?? throw new ArgumentNullException(nameof(resource));
	}

	public uint Version
	{
		get => _resource.Version;
		set => _resource.Version = value;
	}

	public IDictionary<string, IRegionData> Regions
	{
		get
		{
			if (_regionsCache == null)
			{
				_regionsCache = new Dictionary<string, IRegionData>();
				foreach (var kvp in _resource.Regions)
				{
					_regionsCache[kvp.Key] = new WindowsRegionDataAdapter(kvp.Value);
				}
			}
			return _regionsCache;
		}
	}

	public IRegionData GetRegion(string name)
	{
		if (_resource.Regions.TryGetValue(name, out var region))
		{
			return new WindowsRegionDataAdapter(region);
		}
		return null;
	}

	public bool TryGetRegion(string name, out IRegionData region)
	{
		region = GetRegion(name);
		return region != null;
	}

	/// <summary>
	/// Gets the underlying LSLib Resource for Windows-specific operations.
	/// </summary>
	public Resource UnderlyingResource => _resource;
}

#endif
