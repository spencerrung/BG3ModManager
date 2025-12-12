#if WINDOWS || _WINDOWS

using DivinityModManager.Models.Resources;
using LSLib.LS;

namespace DivinityModManager.Services.Windows;

/// <summary>
/// Adapts LSLib.LS.NodeAttribute to IAttributeData interface for Windows.
/// </summary>
internal class WindowsAttributeDataAdapter : IAttributeData
{
	private readonly NodeAttribute _attribute;

	public WindowsAttributeDataAdapter(NodeAttribute attribute)
	{
		_attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
	}

	public string Name => _attribute.Name;
	public object Value
	{
		get => _attribute.Value;
		set => _attribute.Value = value;
	}
	public string TypeName => _attribute.Type?.ToString() ?? "Unknown";

	/// <summary>
	/// Gets the underlying LSLib NodeAttribute for Windows-specific operations.
	/// </summary>
	public NodeAttribute UnderlyingAttribute => _attribute;
}

#endif
