using System.Runtime.InteropServices;
using DivinityModManager.Services;

#if WINDOWS || _WINDOWS
using DivinityModManager.Services.Windows;
#else
using DivinityModManager.Services.Linux;
#endif

namespace DivinityModManager.Platform;

/// <summary>
/// Factory for creating platform-specific resource service implementations.
/// Returns Windows implementation on Windows, Linux stub on Linux.
/// </summary>
public static class ResourceServiceFactory
{
	/// <summary>
	/// Creates a platform-specific IResourceService implementation.
	/// </summary>
	public static IResourceService CreateResourceService()
	{
#if WINDOWS || _WINDOWS
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return new WindowsResourceService();
		}
#else
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return new LinuxResourceService();
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			// macOS also uses stub for now (no LSLib support)
			return new LinuxResourceService();
		}
#endif

		// Fallback to Linux stub for unsupported platforms
		return new LinuxResourceService();
	}
}
