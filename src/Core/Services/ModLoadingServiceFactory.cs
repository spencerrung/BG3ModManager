using System.Runtime.InteropServices;
#if !LINUX_BUILD
using DivinityModManager.Services.Windows;
#endif
using DivinityModManager.Services.Linux;

namespace DivinityModManager.Services;

/// <summary>
/// Factory for creating platform-specific mod loading service implementations.
/// Returns Windows implementation on Windows, Linux stub on Linux.
/// </summary>
public static class ModLoadingServiceFactory
{
	/// <summary>
	/// Creates a platform-specific IModLoadingService implementation.
	/// </summary>
	public static IModLoadingService CreateModLoadingService()
	{
#if !LINUX_BUILD
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return new WindowsModLoadingService();
		}
#endif
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return new LinuxModLoadingService();
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			// macOS also uses stub for now (no LSLib support)
			return new LinuxModLoadingService();
		}

		// Fallback to Linux stub for unsupported platforms
		return new LinuxModLoadingService();
	}
}
