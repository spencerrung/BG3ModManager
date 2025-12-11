using System;
using DivinityModManager.Platform;
using Splat;

namespace DivinityModManager;

/// <summary>
/// Helper class for initializing and registering platform services.
/// Called during application startup to set up cross-platform service access.
/// </summary>
public static class PlatformServiceInitializer
{
	/// <summary>
	/// Initializes and registers platform services in the Splat service locator.
	/// Should be called during application initialization, before any platform services are needed.
	/// </summary>
	public static void RegisterPlatformServices()
	{
		try
		{
			DivinityApp.Log("Initializing platform services...");

			// Create the platform services for the current OS
			var platformServices = PlatformServicesFactory.CreatePlatformServices();

			// Register the aggregator service
			Locator.Current.Register(
				() => platformServices,
				typeof(IPlatformServices)
			);

			// Optionally register individual services for direct injection
			Locator.Current.Register(
				() => platformServices.FileDialogs,
				typeof(IFileDialogService)
			);

			Locator.Current.Register(
				() => platformServices.Registry,
				typeof(IRegistryService)
			);

			Locator.Current.Register(
				() => platformServices.FileSystem,
				typeof(IFileSystemService)
			);

			Locator.Current.Register(
				() => platformServices.Process,
				typeof(IProcessService)
			);

			Locator.Current.Register(
				() => platformServices.Accessibility,
				typeof(IAccessibilityService)
			);

			DivinityApp.Log("Platform services registered successfully");
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Failed to initialize platform services: {ex.Message}");
			throw;
		}
	}

	/// <summary>
	/// Retrieves the registered IPlatformServices instance from the service locator.
	/// </summary>
	/// <returns>The registered IPlatformServices instance</returns>
	public static IPlatformServices GetPlatformServices()
	{
		try
		{
			return Locator.Current.GetService<IPlatformServices>()
				?? throw new InvalidOperationException("Platform services not registered");
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error retrieving platform services: {ex.Message}");
			throw;
		}
	}
}
