using System;
using System.Diagnostics;
using System.IO;

namespace DivinityModManager.Platform.Linux;

/// <summary>
/// Linux implementation of file system service.
/// Provides operations like delete to trash and symlink management.
/// </summary>
public class LinuxFileSystemService : IFileSystemService
{
	public void DeleteToRecycleBin(string path)
	{
		try
		{
			if (!File.Exists(path) && !Directory.Exists(path))
			{
				throw new FileNotFoundException($"Path not found: {path}");
			}

			// Try gio trash first (GNOME/freedesktop standard)
			if (ExecuteCommand("gio", $"trash \"{path}\""))
			{
				return;
			}

			// Fallback to trash-cli if available
			if (ExecuteCommand("trash-put", $"\"{path}\""))
			{
				return;
			}

			// Last resort: remove directly (permanent deletion)
			DivinityApp.Log("Warning: Using permanent deletion instead of trash. Install 'gio' or 'trash-cli' for trash support.");
			if (Directory.Exists(path))
			{
				Directory.Delete(path, recursive: true);
			}
			else
			{
				File.Delete(path);
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error deleting to recycle bin: {ex.Message}");
			throw;
		}
	}

	public void CreateJunctionPoint(string linkPath, string targetPath)
	{
		try
		{
			if (!Directory.Exists(targetPath))
			{
				throw new DirectoryNotFoundException($"Target path not found: {targetPath}");
			}

			if (Directory.Exists(linkPath) || File.Exists(linkPath))
			{
				throw new InvalidOperationException($"Link path already exists: {linkPath}");
			}

			// Get absolute path for target
			var absoluteTargetPath = Path.GetFullPath(targetPath);

			// Create symbolic link using ln -s
			if (!ExecuteCommand("ln", $"-s \"{absoluteTargetPath}\" \"{linkPath}\""))
			{
				throw new InvalidOperationException($"Failed to create symbolic link via ln command");
			}

			DivinityApp.Log($"Junction point created: {linkPath} -> {absoluteTargetPath}");
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error creating junction point: {ex.Message}");
			throw;
		}
	}

	public void DeleteJunctionPoint(string linkPath)
	{
		try
		{
			if (!Directory.Exists(linkPath))
			{
				throw new DirectoryNotFoundException($"Path not found: {linkPath}");
			}

			// Check if it's actually a symlink
			if (!IsJunctionPoint(linkPath))
			{
				throw new InvalidOperationException($"Path is not a symbolic link: {linkPath}");
			}

			// Remove the symlink (not the target)
			if (!ExecuteCommand("rm", $"\"{linkPath}\""))
			{
				throw new InvalidOperationException($"Failed to delete symbolic link via rm command");
			}

			DivinityApp.Log($"Junction point deleted: {linkPath}");
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error deleting junction point: {ex.Message}");
			throw;
		}
	}

	public bool IsJunctionPoint(string path)
	{
		try
		{
			if (!Directory.Exists(path))
			{
				return false;
			}

			// Use 'test -L' to check if path is a symlink
			var startInfo = new ProcessStartInfo
			{
				FileName = "test",
				Arguments = $"-L \"{path}\"",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			using (var process = Process.Start(startInfo))
			{
				if (process == null)
				{
					return false;
				}

				process.WaitForExit();
				return process.ExitCode == 0;
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error determining if path is junction point: {ex.Message}");
			return false;
		}
	}

	private bool ExecuteCommand(string command, string arguments)
	{
		try
		{
			var startInfo = new ProcessStartInfo
			{
				FileName = command,
				Arguments = arguments,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			using (var process = Process.Start(startInfo))
			{
				if (process == null)
				{
					DivinityApp.Log($"Failed to start process: {command}");
					return false;
				}

				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					var error = process.StandardError.ReadToEnd();
					DivinityApp.Log($"Command '{command}' failed with exit code {process.ExitCode}: {error}");
					return false;
				}

				return true;
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error executing command '{command}': {ex.Message}");
			return false;
		}
	}
}
