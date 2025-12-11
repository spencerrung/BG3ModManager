using System;
using System.Threading.Tasks;

namespace DivinityModManager.Platform.Windows;

/// <summary>
/// Windows implementation of accessibility service.
/// Provides screen reader announcements and accessibility features.
/// Uses the existing ScreenReaderService and CrossSpeak integration from the project.
/// </summary>
public class WindowsAccessibilityService : IAccessibilityService
{
	private IScreenReaderService? _screenReaderService;

	public WindowsAccessibilityService()
	{
		try
		{
			// Try to get the existing ScreenReaderService from the app if available
			_screenReaderService = DivinityApp.ScreenReaderService;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Failed to initialize ScreenReaderService: {ex.Message}");
			_screenReaderService = null;
		}
	}

	public void Announce(string message)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}

			if (_screenReaderService != null)
			{
				_screenReaderService.Announce(message);
			}
			else
			{
				DivinityApp.Log($"Screen reader service not available, cannot announce: {message}");
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error announcing message: {ex.Message}");
		}
	}

	public async Task AnnounceAsync(string message)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}

			// Wrap synchronous announcement in a task
			await Task.Run(() =>
			{
				if (_screenReaderService != null)
				{
					_screenReaderService.Announce(message);
				}
				else
				{
					DivinityApp.Log($"Screen reader service not available, cannot announce: {message}");
				}
			});
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error announcing message asynchronously: {ex.Message}");
		}
	}
}
