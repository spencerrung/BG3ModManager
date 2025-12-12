using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DivinityModManager.Platform.Linux;

/// <summary>
/// Linux implementation of file dialog service using Zenity or KDialog.
/// Falls back to command-line prompts if graphical dialogs are unavailable.
/// </summary>
public class LinuxFileDialogService : IFileDialogService
{
	private readonly string _dialogTool;

	public LinuxFileDialogService()
	{
		// Detect available dialog tool: prefer zenity (GNOME), fallback to kdialog (KDE), then Xdialog
		_dialogTool = DetectDialogTool();
		if (string.IsNullOrEmpty(_dialogTool))
		{
			DivinityApp.Log("Warning: No graphical dialog tool detected (zenity, kdialog, or Xdialog required)");
		}
	}

	public async Task<string?> OpenFileAsync(string title, string filter)
	{
		var result = await Task.Run(() => OpenFileInternal(title, filter));
		return result;
	}

	public async Task<IReadOnlyList<string>> OpenFilesAsync(string title, string filter)
	{
		var result = await Task.Run(() => OpenFilesInternal(title, filter));
		return result;
	}

	public async Task<string?> OpenFolderAsync(string title)
	{
		var result = await Task.Run(() => OpenFolderInternal(title));
		return result;
	}

	public async Task<string?> SaveFileAsync(string title, string filter, string suggestedFileName)
	{
		var result = await Task.Run(() => SaveFileInternal(title, filter, suggestedFileName));
		return result;
	}

	private string? OpenFileInternal(string title, string filter)
	{
		try
		{
			if (string.IsNullOrEmpty(_dialogTool))
			{
				DivinityApp.Log("No dialog tool available for file selection");
				return null;
			}

			var filterArg = ConvertFilterForZenity(filter);
			var args = $"--file-selection --title=\"{EscapeArg(title)}\" {filterArg}";

			var output = ExecuteDialog(args);
			return string.IsNullOrWhiteSpace(output) ? null : output.Trim();
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error opening file dialog: {ex.Message}");
			return null;
		}
	}

	private IReadOnlyList<string> OpenFilesInternal(string title, string filter)
	{
		try
		{
			if (string.IsNullOrEmpty(_dialogTool))
			{
				DivinityApp.Log("No dialog tool available for file selection");
				return new List<string>();
			}

			var filterArg = ConvertFilterForZenity(filter);
			var args = $"--file-selection --multiple --title=\"{EscapeArg(title)}\" {filterArg}";

			var output = ExecuteDialog(args);
			if (string.IsNullOrWhiteSpace(output))
			{
				return new List<string>();
			}

			// Zenity returns multiple files separated by pipe character in multi-select mode
			return output.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.ToList();
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error opening files dialog: {ex.Message}");
			return new List<string>();
		}
	}

	private string? OpenFolderInternal(string title)
	{
		try
		{
			if (string.IsNullOrEmpty(_dialogTool))
			{
				DivinityApp.Log("No dialog tool available for folder selection");
				return null;
			}

			var args = $"--file-selection --directory --title=\"{EscapeArg(title)}\"";

			var output = ExecuteDialog(args);
			return string.IsNullOrWhiteSpace(output) ? null : output.Trim();
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error opening folder dialog: {ex.Message}");
			return null;
		}
	}

	private string? SaveFileInternal(string title, string filter, string suggestedFileName)
	{
		try
		{
			if (string.IsNullOrEmpty(_dialogTool))
			{
				DivinityApp.Log("No dialog tool available for save dialog");
				return null;
			}

			var filterArg = ConvertFilterForZenity(filter);
			var filename = string.IsNullOrEmpty(suggestedFileName) ? "" : $"--filename=\"{EscapeArg(suggestedFileName)}\"";
			var args = $"--file-selection --save --title=\"{EscapeArg(title)}\" {filename} {filterArg}";

			var output = ExecuteDialog(args);
			return string.IsNullOrWhiteSpace(output) ? null : output.Trim();
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error opening save dialog: {ex.Message}");
			return null;
		}
	}

	private string ExecuteDialog(string args)
	{
		try
		{
			var startInfo = new ProcessStartInfo
			{
				FileName = _dialogTool,
				Arguments = args,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			using (var process = Process.Start(startInfo))
			{
				if (process == null)
				{
					throw new InvalidOperationException($"Failed to start {_dialogTool} process");
				}

				var output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();

				// Dialog was cancelled (exit code 1) or error
				if (process.ExitCode != 0)
				{
					return null;
				}

				return output;
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error executing dialog tool: {ex.Message}");
			return null;
		}
	}

	private string DetectDialogTool()
	{
		var tools = new[] { "zenity", "kdialog", "Xdialog" };

		foreach (var tool in tools)
		{
			try
			{
				var startInfo = new ProcessStartInfo
				{
					FileName = "which",
					Arguments = tool,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				};

				using (var process = Process.Start(startInfo))
				{
					if (process == null) continue;

					process.WaitForExit();
					if (process.ExitCode == 0)
					{
						DivinityApp.Log($"Detected dialog tool: {tool}");
						return tool;
					}
				}
			}
			catch (Exception ex)
			{
				DivinityApp.Log($"Error detecting {tool}: {ex.Message}");
			}
		}

		return null;
	}

	private string ConvertFilterForZenity(string filter)
	{
		// Convert pipe-delimited filter format to zenity format
		// Input: "Text Files|*.txt|All Files|*.*"
		// Output: --file-filter="Text Files (*.txt)" --file-filter="All Files (*.*)"

		if (string.IsNullOrEmpty(filter))
		{
			return "";
		}

		var parts = filter.Split('|');
		var result = new List<string>();

		for (int i = 0; i < parts.Length; i += 2)
		{
			if (i + 1 < parts.Length)
			{
				var name = parts[i];
				var pattern = parts[i + 1];
				result.Add($"--file-filter=\"{name} ({pattern})\"");
			}
		}

		return string.Join(" ", result);
	}

	private string EscapeArg(string arg)
	{
		// Escape quotes and special characters for shell
		return arg.Replace("\"", "\\\"").Replace("'", "\\'");
	}
}
