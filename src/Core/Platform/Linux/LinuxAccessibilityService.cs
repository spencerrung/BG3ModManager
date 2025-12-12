using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DivinityModManager.Platform.Linux;

/// <summary>
/// Linux implementation of accessibility service.
/// Provides screen reader announcements using Speech Dispatcher or espeak.
/// </summary>
public class LinuxAccessibilityService : IAccessibilityService
{
	private readonly string? _speechTool;

	public LinuxAccessibilityService()
	{
		// Detect available speech tool
		_speechTool = DetectSpeechTool();
		if (string.IsNullOrEmpty(_speechTool))
		{
			DivinityApp.Log("Warning: No speech synthesis tool detected (spd-say or espeak required for accessibility)");
		}
	}

	public void Announce(string message)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}

			if (string.IsNullOrEmpty(_speechTool))
			{
				DivinityApp.Log($"Speech synthesis not available, cannot announce: {message}");
				return;
			}

			// Execute speech command without waiting
			var startInfo = new ProcessStartInfo
			{
				FileName = _speechTool,
				Arguments = $"\"{EscapeArg(message)}\"",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			using (var process = Process.Start(startInfo))
			{
				if (process == null)
				{
					DivinityApp.Log($"Failed to start speech tool: {_speechTool}");
				}
				// Don't wait for process to complete (fire and forget)
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error announcing message: {ex.Message}");
		}
	}

	public async Task AnnounceAsync(string message)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}

			if (string.IsNullOrEmpty(_speechTool))
			{
				DivinityApp.Log($"Speech synthesis not available, cannot announce: {message}");
				return;
			}

			// Run speech synthesis asynchronously
			await Task.Run(() =>
			{
				try
				{
					var startInfo = new ProcessStartInfo
					{
						FileName = _speechTool,
						Arguments = $"\"{EscapeArg(message)}\"",
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						CreateNoWindow = true
					};

					using (var process = Process.Start(startInfo))
					{
						if (process == null)
						{
							DivinityApp.Log($"Failed to start speech tool: {_speechTool}");
							return;
						}

						// Wait for process to complete
						process.WaitForExit(10000); // 10 second timeout
					}
				}
				catch (Exception ex)
				{
					DivinityApp.Log($"Error in async announcement: {ex.Message}");
				}
			});
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error announcing message asynchronously: {ex.Message}");
		}
	}

	private string? DetectSpeechTool()
	{
		var tools = new[] { "spd-say", "espeak", "espeak-ng" };

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
						DivinityApp.Log($"Detected speech tool: {tool}");
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

	private string EscapeArg(string arg)
	{
		// Escape quotes and special characters for shell
		return arg.Replace("\"", "\\\"").Replace("'", "\\'").Replace("`", "\\`");
	}
}
