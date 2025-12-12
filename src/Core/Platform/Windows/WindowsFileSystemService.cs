using System;
using System.IO;

namespace DivinityModManager.Platform.Windows;

/// <summary>
/// Windows implementation of file system service.
/// Provides operations like delete to recycle bin and junction point management.
/// </summary>
public class WindowsFileSystemService : IFileSystemService
{
	public void DeleteToRecycleBin(string path)
	{
		try
		{
			if (!File.Exists(path) && !Directory.Exists(path))
			{
				throw new FileNotFoundException($"Path not found: {path}");
			}

			// Use the existing RecycleBinHelper if available, otherwise implement via P/Invoke
			try
			{
				// Try to use existing utility if it exists in the project
				var recycleBinHelperType = Type.GetType("DivinityModManager.RecycleBinHelper");
				if (recycleBinHelperType != null)
				{
					var method = recycleBinHelperType.GetMethod("DeleteFile",
						new[] { typeof(string) });
					if (method != null)
					{
						method.Invoke(null, new object[] { path });
						return;
					}
				}

				// Fallback: Use SHFileOperation via P/Invoke
				DeleteToRecycleBinViaApi(path);
			}
			catch (Exception ex)
			{
				DivinityApp.Log($"Error using RecycleBinHelper, falling back to API: {ex.Message}");
				DeleteToRecycleBinViaApi(path);
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

			// Try to use existing JunctionPoint utility if available
			try
			{
				var junctionPointType = Type.GetType("DivinityModManager.JunctionPoint");
				if (junctionPointType != null)
				{
					var method = junctionPointType.GetMethod("Create",
						new[] { typeof(string), typeof(string) });
					if (method != null)
					{
						method.Invoke(null, new object[] { linkPath, targetPath });
						return;
					}
				}

				// Fallback: Use mklink command
				CreateJunctionPointViaCmd(linkPath, targetPath);
			}
			catch (Exception ex)
			{
				DivinityApp.Log($"Error using JunctionPoint utility, falling back to cmd: {ex.Message}");
				CreateJunctionPointViaCmd(linkPath, targetPath);
			}
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

			// Try to use existing JunctionPoint utility if available
			try
			{
				var junctionPointType = Type.GetType("DivinityModManager.JunctionPoint");
				if (junctionPointType != null)
				{
					var method = junctionPointType.GetMethod("Delete",
						new[] { typeof(string) });
					if (method != null)
					{
						method.Invoke(null, new object[] { linkPath });
						return;
					}
				}

				// Fallback: Use rmdir command
				DeleteJunctionPointViaCmd(linkPath);
			}
			catch (Exception ex)
			{
				DivinityApp.Log($"Error using JunctionPoint utility, falling back to cmd: {ex.Message}");
				DeleteJunctionPointViaCmd(linkPath);
			}
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

			// Try to use existing JunctionPoint utility if available
			try
			{
				var junctionPointType = Type.GetType("DivinityModManager.JunctionPoint");
				if (junctionPointType != null)
				{
					var method = junctionPointType.GetMethod("Exists",
						new[] { typeof(string) });
					if (method != null)
					{
						var result = method.Invoke(null, new object[] { path });
						return (bool)result;
					}
				}

				// Fallback: Check using DirectoryInfo attributes
				var dirInfo = new DirectoryInfo(path);
				return (dirInfo.Attributes & FileAttributes.ReparsePoint) != 0;
			}
			catch (Exception ex)
			{
				DivinityApp.Log($"Error checking if junction point, falling back to attributes: {ex.Message}");
				var dirInfo = new DirectoryInfo(path);
				return (dirInfo.Attributes & FileAttributes.ReparsePoint) != 0;
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error determining if path is junction point: {ex.Message}");
			return false;
		}
	}

	private void DeleteToRecycleBinViaApi(string path)
	{
		// Use SHFileOperation for moving files to recycle bin
		// This requires P/Invoke for the Shell API
		var fileOperation = new SHFILEOPSTRUCT
		{
			wFunc = FileOperationType.FO_DELETE,
			pFrom = path + "\0\0",
			fFlags = FileOperationFlags.FOF_ALLOWUNDO | FileOperationFlags.FOF_NOCONFIRMATION
		};

		int result = SHFileOperation(ref fileOperation);
		if (result != 0)
		{
			throw new InvalidOperationException($"SHFileOperation failed with code {result}");
		}
	}

	private void CreateJunctionPointViaCmd(string linkPath, string targetPath)
	{
		try
		{
			var startInfo = new System.Diagnostics.ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = $"/c mklink /J \"{linkPath}\" \"{targetPath}\"",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			using (var process = System.Diagnostics.Process.Start(startInfo))
			{
				if (process == null)
				{
					throw new InvalidOperationException("Failed to start mklink process");
				}

				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					var error = process.StandardError.ReadToEnd();
					throw new InvalidOperationException($"mklink command failed: {error}");
				}
			}
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to create junction point via mklink: {ex.Message}", ex);
		}
	}

	private void DeleteJunctionPointViaCmd(string linkPath)
	{
		try
		{
			var startInfo = new System.Diagnostics.ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = $"/c rmdir /s /q \"{linkPath}\"",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			using (var process = System.Diagnostics.Process.Start(startInfo))
			{
				if (process == null)
				{
					throw new InvalidOperationException("Failed to start rmdir process");
				}

				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					var error = process.StandardError.ReadToEnd();
					throw new InvalidOperationException($"rmdir command failed: {error}");
				}
			}
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to delete junction point via rmdir: {ex.Message}", ex);
		}
	}

	// P/Invoke structures for SHFileOperation
	[System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
	private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
	private struct SHFILEOPSTRUCT
	{
		public IntPtr hwnd;
		[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
		public FileOperationType wFunc;
		public string pFrom;
		public string pTo;
		public FileOperationFlags fFlags;
		[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
		public bool fAnyOperationsAborted;
		public IntPtr hNameMappings;
		public string lpszProgressTitle;
	}

	[System.Runtime.InteropServices.Flags]
	private enum FileOperationType : uint
	{
		FO_MOVE = 0x0001,
		FO_COPY = 0x0002,
		FO_DELETE = 0x0003,
		FO_RENAME = 0x0004
	}

	[System.Runtime.InteropServices.Flags]
	private enum FileOperationFlags : ushort
	{
		FOF_MULTIDESTFILES = 0x0001,
		FOF_CONFIRMMOUSE = 0x0002,
		FOF_SILENT = 0x0004,
		FOF_RENAMEONCOLLISION = 0x0008,
		FOF_NOCONFIRMATION = 0x0010,
		FOF_WANTMAPPINGHANDLES = 0x0020,
		FOF_ALLOWUNDO = 0x0040,
		FOF_FILESONLY = 0x0080,
		FOF_SIMPLEPROGRESS = 0x0100,
		FOF_NOCONFIRMMKDIR = 0x0200,
		FOF_NOERRORUI = 0x0400,
		FOF_NOCOPYSECURITYATTRIBS = 0x0800,
		FOF_NORECURSION = 0x1000,
		FOF_NO_CONNECTED_ELEMENTS = 0x2000,
		FOF_WANT_MAPPED_HANDLE = 0x0020,
		FOF_NO_UI = 0x0614
	}
}
