using System;
using System.Diagnostics;

namespace DivinityModManager.Platform.Windows;

/// <summary>
/// Windows implementation of process service.
/// Handles launching games, opening URLs, and opening folders.
/// </summary>
public class WindowsProcessService : IProcessService
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
				UseShellExecute = true,
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

			var startInfo = new ProcessStartInfo
			{
				FileName = url,
				UseShellExecute = true,
				CreateNoWindow = true
			};

			using (var process = Process.Start(startInfo))
			{
				if (process == null)
				{
					throw new InvalidOperationException("Failed to open URL");
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

			// Get the full path and normalize it
			var fullPath = Path.GetFullPath(folderPath);

			// Use explorer.exe to open the folder
			var startInfo = new ProcessStartInfo
			{
				FileName = "explorer.exe",
				Arguments = $"/select,\"{fullPath}\"",
				UseShellExecute = true,
				CreateNoWindow = true
			};

			using (var process = Process.Start(startInfo))
			{
				if (process == null)
				{
					throw new InvalidOperationException("Failed to open folder");
				}

				DivinityApp.Log($"Folder opened: {folderPath}");
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error opening folder: {ex.Message}");
			throw;
		}
	}
}
