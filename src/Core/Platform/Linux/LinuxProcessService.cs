using System;
using System.Diagnostics;

namespace DivinityModManager.Platform.Linux;

/// <summary>
/// Linux implementation of process service.
/// Handles launching games, opening URLs, and opening folders.
/// </summary>
public class LinuxProcessService : IProcessService
{
	public void LaunchGame(string processPath, string arguments)
	{
		try
		{
			if (!File.Exists(processPath))
			{
				throw new FileNotFoundException($"Process not found: {processPath}");
			}

			var startInfo = new ProcessStartInfo
			{
				FileName = processPath,
				Arguments = arguments,
				UseShellExecute = false,
				CreateNoWindow = false
			};

			using (var process = Process.Start(startInfo))
			{
				if (process == null)
				{
					throw new InvalidOperationException("Failed to start game process");
				}

				DivinityApp.Log($"Game process started: {processPath}");
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error launching game: {ex.Message}");
			throw;
		}
	}

	public void OpenUrl(string url)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				throw new ArgumentException("URL cannot be empty", nameof(url));
			}

			// Use xdg-open to open URL with default browser
			var startInfo = new ProcessStartInfo
			{
				FileName = "xdg-open",
				Arguments = url,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			using (var process = Process.Start(startInfo))
			{
				if (process == null)
				{
					throw new InvalidOperationException("Failed to open URL");
				}

				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					var error = process.StandardError.ReadToEnd();
					DivinityApp.Log($"xdg-open failed with code {process.ExitCode}: {error}");
				}

				DivinityApp.Log($"URL opened: {url}");
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error opening URL: {ex.Message}");
			throw;
		}
	}

	public void OpenFolder(string folderPath)
	{
		try
		{
			if (!Directory.Exists(folderPath))
			{
				throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
			}

			// Get the full path
			var fullPath = Path.GetFullPath(folderPath);

			// Try different file managers in order of preference
			var fileManagers = new[] { "xdg-open", "nautilus", "dolphin", "thunar", "pcmanfm", "nemo" };

			foreach (var manager in fileManagers)
			{
				try
				{
					var startInfo = new ProcessStartInfo
					{
						FileName = manager,
						Arguments = $"\"{fullPath}\"",
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						CreateNoWindow = true
					};

					using (var process = Process.Start(startInfo))
					{
						if (process == null)
						{
							continue;
						}

						// Don't wait for xdg-open, as it returns immediately
						if (manager != "xdg-open")
						{
							process.WaitForExit(2000); // Wait max 2 seconds
						}

						if (process.ExitCode == 0 || manager == "xdg-open")
						{
							DivinityApp.Log($"Folder opened with {manager}: {folderPath}");
							return;
						}
					}
				}
				catch
				{
					// Try next file manager
					continue;
				}
			}

			throw new InvalidOperationException("Could not find a file manager to open the folder");
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error opening folder: {ex.Message}");
			throw;
		}
	}
}
