using System;

namespace DivinityModManager.Platform.Windows;

/// <summary>
/// Windows platform services aggregator.
/// Combines all Windows-specific service implementations into a single interface.
/// </summary>
public class WindowsPlatformServices : IPlatformServices
{
	public IFileDialogService FileDialogs { get; }
	public IRegistryService Registry { get; }
	public IFileSystemService FileSystem { get; }
	public IProcessService Process { get; }
	public IAccessibilityService Accessibility { get; }

	public WindowsPlatformServices()
	{
		try
		{
			FileDialogs = new WindowsFileDialogService();
			Registry = new WindowsRegistryService();
			FileSystem = new WindowsFileSystemService();
			Process = new WindowsProcessService();
			Accessibility = new WindowsAccessibilityService();

			DivinityApp.Log("Windows platform services initialized successfully");
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error initializing Windows platform services: {ex.Message}");
			throw;
		}
	}
}
