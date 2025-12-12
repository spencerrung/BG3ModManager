#if WINDOWS || _WINDOWS

using DivinityModManager.Models.Resources;
using LSLib.LS;

namespace DivinityModManager.Services.Windows;

/// <summary>
/// Adapts LSLib.LS.Region to IRegionData interface for Windows.
/// </summary>
internal class WindowsRegionDataAdapter : IRegionData
{
	private readonly Region _region;
	private Dictionary<string, List<INodeData>> _childrenCache;

	public WindowsRegionDataAdapter(Region region)
	{
		_region = region ?? throw new ArgumentNullException(nameof(region));
	}

	public string Name
	{
		get => _region.Name;
		set => _region.Name = value;
	}

	public string RegionName => _region.RegionName;

	public IDictionary<string, List<INodeData>> Children
	{
		get
		{
			if (_childrenCache == null)
			{
				_childrenCache = new Dictionary<string, List<INodeData>>();
				foreach (var kvp in _region.Children)
				{
					_childrenCache[kvp.Key] = kvp.Value.Select(n => new WindowsNodeDataAdapter(n) as INodeData).ToList();
				}
			}
			return _childrenCache;
		}
	}

	/// <summary>
	/// Gets the underlying LSLib Region for Windows-specific operations.
	/// </summary>
	public Region UnderlyingRegion => _region;
}

#endif
