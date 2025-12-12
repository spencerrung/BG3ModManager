#if !WINDOWS && !(_WINDOWS)

using DivinityModManager.Models.Resources;

namespace DivinityModManager.Services.Linux;

/// <summary>
/// Linux stub implementation of INodeData.
/// Returns empty structures - no actual resource parsing on Linux.
/// </summary>
internal class LinuxNodeDataStub : INodeData
{
	private Dictionary<string, IAttributeData> _attributes = [];
	private Dictionary<string, List<INodeData>> _children = [];

	public LinuxNodeDataStub(string name = "")
	{
		Name = name;
	}

	public string Name { get; set; }
	public INodeData Parent { get; set; }
	public IDictionary<string, IAttributeData> Attributes => _attributes;
	public IDictionary<string, List<INodeData>> Children => _children;
	public int ChildCount => _children.Values.Sum(l => l.Count);

	public void AppendChild(INodeData child)
	{
		if (child == null) return;

		if (!_children.ContainsKey(child.Name))
		{
			_children[child.Name] = [];
		}
		_children[child.Name].Add(child);
	}

	public INodeData GetChild(string name, int index = 0)
	{
		if (_children.TryGetValue(name, out var nodes) && nodes.Count > index)
		{
			return nodes[index];
		}
		return null;
	}
}

#endif
