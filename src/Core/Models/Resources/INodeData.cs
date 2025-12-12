namespace DivinityModManager.Models.Resources;

/// <summary>
/// Platform-agnostic interface for resource nodes.
/// Abstracts away LSLib.LS.Node to allow cross-platform builds.
/// </summary>
public interface INodeData
{
	string Name { get; set; }
	INodeData Parent { get; set; }
	IDictionary<string, IAttributeData> Attributes { get; }
	IDictionary<string, List<INodeData>> Children { get; }
	int ChildCount { get; }

	void AppendChild(INodeData child);
	INodeData GetChild(string name, int index = 0);
}
