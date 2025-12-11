using DivinityModManager.Enums;

namespace DivinityModManager.Models;

public interface ISelectable
{
	bool IsSelected { get; set; }
	UIVisibility Visibility { get; set; }
	bool CanDrag { get; }
}
