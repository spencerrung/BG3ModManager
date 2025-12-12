#if WINDOWS || _WINDOWS

using DivinityModManager.Models.Resources;
using LSLib.LS;

namespace DivinityModManager.Services.Windows;

/// <summary>
/// Adapts LSLib.LS.Node to INodeData interface for Windows.
/// </summary>
internal class WindowsNodeDataAdapter : INodeData
{
	private readonly Node _node;
	private Dictionary<string, IAttributeData> _attributesCache;
	private Dictionary<string, List<INodeData>> _childrenCache;

	public WindowsNodeDataAdapter(Node node)
	{
		_node = node ?? throw new ArgumentNullException(nameof(node));
	}

	public string Name
	{
		get => _node.Name;
		set => _node.Name = value;
	}

	public INodeData Parent
	{
		get => _node.Parent != null ? new WindowsNodeDataAdapter(_node.Parent) : null;
		set => _node.Parent = (value as WindowsNodeDataAdapter)?._node;
	}

	public IDictionary<string, IAttributeData> Attributes
	{
		get
		{
			if (_attributesCache == null)
			{
				_attributesCache = new Dictionary<string, IAttributeData>();
				foreach (var kvp in _node.Attributes)
				{
					_attributesCache[kvp.Key] = new WindowsAttributeDataAdapter(kvp.Value);
				}
			}
			return _attributesCache;
		}
	}

	public IDictionary<string, List<INodeData>> Children
	{
		get
		{
			if (_childrenCache == null)
			{
				_childrenCache = new Dictionary<string, List<INodeData>>();
				foreach (var kvp in _node.Children)
				{
					_childrenCache[kvp.Key] = kvp.Value.Select(n => new WindowsNodeDataAdapter(n) as INodeData).ToList();
				}
			}
			return _childrenCache;
		}
	}

	public int ChildCount => _node.ChildCount;

	public void AppendChild(INodeData child)
	{
		if (child is WindowsNodeDataAdapter adapter)
		{
			_node.AppendChild(adapter._node);
			_childrenCache = null; // Invalidate cache
		}
		else
		{
			throw new ArgumentException("Child must be a Windows node adapter", nameof(child));
		}
	}

	public INodeData GetChild(string name, int index = 0)
	{
		var children = _node.Children;
		if (children.TryGetValue(name, out var nodeList) && nodeList.Count > index)
		{
			return new WindowsNodeDataAdapter(nodeList[index]);
		}
		return null;
	}

	/// <summary>
	/// Gets the underlying LSLib Node for Windows-specific operations.
	/// </summary>
	public Node UnderlyingNode => _node;
}

#endif
