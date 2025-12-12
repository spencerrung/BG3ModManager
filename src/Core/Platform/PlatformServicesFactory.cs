using System;
using System.Runtime.InteropServices;
#if !LINUX_BUILD
using DivinityModManager.Platform.Windows;
#endif
using DivinityModManager.Platform.Linux;

namespace DivinityModManager.Platform;

/// <summary>
/// Factory for creating platform-specific service implementations.
/// Detects the current operating system and returns appropriate implementations.
/// </summary>
public static class PlatformServicesFactory
{
	/// <summary>
	/// Creates the appropriate platform services instance for the current OS.
	/// </summary>
	/// <returns>IPlatformServices implementation for the current platform</returns>
	/// <exception cref="PlatformNotSupportedException">Thrown if the OS is not supported</exception>
	public static IPlatformServices CreatePlatformServices()
	{
		try
		{
#if !LINUX_BUILD
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				DivinityApp.Log("Detected Windows platform, creating Windows platform services");
				return new WindowsPlatformServices();
			}
#endif
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				DivinityApp.Log("Detected Linux platform, creating Linux platform services");
				return new LinuxPlatformServices();
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				DivinityApp.Log("macOS platform detected but not yet supported");
				throw new PlatformNotSupportedException("macOS support is not yet implemented");
			}
			else
			{
				throw new PlatformNotSupportedException("Unsupported operating system");
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error creating platform services: {ex.Message}");
			throw;
		}
	}
}
