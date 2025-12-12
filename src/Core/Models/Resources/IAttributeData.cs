namespace DivinityModManager.Models.Resources;

/// <summary>
/// Platform-agnostic interface for resource node attributes.
/// Abstracts away LSLib.LS.NodeAttribute to allow cross-platform builds.
/// </summary>
public interface IAttributeData
{
	string Name { get; }
	object Value { get; set; }
	string TypeName { get; }
}
