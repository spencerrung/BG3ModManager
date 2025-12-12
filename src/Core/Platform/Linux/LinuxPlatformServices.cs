using System;

namespace DivinityModManager.Platform.Linux;

/// <summary>
/// Linux platform services aggregator.
/// Combines all Linux-specific service implementations into a single interface.
/// </summary>
public class LinuxPlatformServices : IPlatformServices
{
	public IFileDialogService FileDialogs { get; }
	public IRegistryService Registry { get; }
	public IFileSystemService FileSystem { get; }
	public IProcessService Process { get; }
	public IAccessibilityService Accessibility { get; }

	public LinuxPlatformServices()
	{
		try
		{
			FileDialogs = new LinuxFileDialogService();
			Registry = new LinuxRegistryService();
			FileSystem = new LinuxFileSystemService();
			Process = new LinuxProcessService();
			Accessibility = new LinuxAccessibilityService();

			DivinityApp.Log("Linux platform services initialized successfully");
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error initializing Linux platform services: {ex.Message}");
			throw;
		}
	}
}
