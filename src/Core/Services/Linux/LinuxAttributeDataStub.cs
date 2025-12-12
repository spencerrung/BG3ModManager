#if !WINDOWS && !(_WINDOWS)

using DivinityModManager.Models.Resources;

namespace DivinityModManager.Services.Linux;

/// <summary>
/// Linux stub implementation of IAttributeData.
/// Returns placeholder values - no actual resource parsing on Linux.
/// </summary>
internal class LinuxAttributeDataStub : IAttributeData
{
	public LinuxAttributeDataStub(string name = "", object value = null, string typeName = "Unknown")
	{
		Name = name;
		Value = value;
		TypeName = typeName;
	}

	public string Name { get; set; }
	public object Value { get; set; }
	public string TypeName { get; set; }
}

#endif
