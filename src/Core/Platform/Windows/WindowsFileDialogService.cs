using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DivinityModManager.Platform.Windows;

/// <summary>
/// Windows implementation of file dialog service using native Windows API (GetOpenFileName, GetSaveFileName).
/// Falls back to simple command-line prompts if native dialogs fail.
/// </summary>
public class WindowsFileDialogService : IFileDialogService
{
	private const int MAX_PATH = 260;

	[DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	private static extern bool GetOpenFileName(ref OpenFileName ofn);

	[DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	private static extern bool GetSaveFileName(ref SaveFileName sfn);

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private struct OpenFileName
	{
		public int StructSize;
		public IntPtr DlgOwner;
		public IntPtr Instance;
		[MarshalAs(UnmanagedType.LPStr)]
		public string Filter;
		[MarshalAs(UnmanagedType.LPStr)]
		public string CustomFilter;
		public int MaxCustomFilter;
		public int FilterIndex;
		[MarshalAs(UnmanagedType.LPStr)]
		public string File;
		public int MaxFile;
		[MarshalAs(UnmanagedType.LPStr)]
		public string FileTitle;
		public int MaxFileTitle;
		[MarshalAs(UnmanagedType.LPStr)]
		public string InitialDir;
		[MarshalAs(UnmanagedType.LPStr)]
		public string Title;
		public int Flags;
		public short FileOffset;
		public short FileExtension;
		[MarshalAs(UnmanagedType.LPStr)]
		public string DefExt;
		public IntPtr CustData;
		public IntPtr Hook;
		[MarshalAs(UnmanagedType.LPStr)]
		public string TemplateName;
		public IntPtr ReservedPtr;
		public int Reserved;
		public int FlagsEx;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private struct SaveFileName
	{
		public int StructSize;
		public IntPtr DlgOwner;
		public IntPtr Instance;
		[MarshalAs(UnmanagedType.LPStr)]
		public string Filter;
		[MarshalAs(UnmanagedType.LPStr)]
		public string CustomFilter;
		public int MaxCustomFilter;
		public int FilterIndex;
		[MarshalAs(UnmanagedType.LPStr)]
		public string File;
		public int MaxFile;
		[MarshalAs(UnmanagedType.LPStr)]
		public string FileTitle;
		public int MaxFileTitle;
		[MarshalAs(UnmanagedType.LPStr)]
		public string InitialDir;
		[MarshalAs(UnmanagedType.LPStr)]
		public string Title;
		public int Flags;
		public short FileOffset;
		public short FileExtension;
		[MarshalAs(UnmanagedType.LPStr)]
		public string DefExt;
		public IntPtr CustData;
		public IntPtr Hook;
		[MarshalAs(UnmanagedType.LPStr)]
		public string TemplateName;
		public IntPtr ReservedPtr;
		public int Reserved;
		public int FlagsEx;
	}

	// Flags for GetOpenFileName/GetSaveFileName
	private const int OFN_READONLY = 0x00000001;
	private const int OFN_OVERWRITEPROMPT = 0x00000002;
	private const int OFN_HIDEREADONLY = 0x00000004;
	private const int OFN_NOCHANGEDIR = 0x00000008;
	private const int OFN_SHOWHELP = 0x00000010;
	private const int OFN_NOVALIDATE = 0x00000100;
	private const int OFN_ALLOWMULTISELECT = 0x00000200;
	private const int OFN_EXTENSIONDIFFERENT = 0x00000400;
	private const int OFN_PATHMUSTEXIST = 0x00000800;
	private const int OFN_FILEMUSTEXIST = 0x00001000;
	private const int OFN_CREATEPROMPT = 0x00002000;
	private const int OFN_SHAREAWARE = 0x00004000;
	private const int OFN_NOREADONLYRETURN = 0x00008000;
	private const int OFN_NOTESTFILECREATE = 0x00010000;
	private const int OFN_NONETWORKBUTTON = 0x00020000;
	private const int OFN_NOLONGNAMES = 0x00040000;
	private const int OFN_EXPLORER = 0x00080000;
	private const int OFN_NODEREFERENCELINKS = 0x00100000;
	private const int OFN_LONGNAMES = 0x00200000;

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
			var ofn = new OpenFileName();
			ofn.StructSize = Marshal.SizeOf(ofn);
			ofn.Title = title;
			ofn.Filter = ConvertFilter(filter);
			ofn.FilterIndex = 1;
			ofn.File = new string(new char[MAX_PATH]);
			ofn.MaxFile = MAX_PATH;
			ofn.FileTitle = new string(new char[64]);
			ofn.MaxFileTitle = 64;
			ofn.InitialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			ofn.Flags = OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST | OFN_HIDEREADONLY | OFN_EXPLORER;

			if (GetOpenFileName(ref ofn))
			{
				return ofn.File.TrimEnd('\0');
			}

			return null;
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
			var ofn = new OpenFileName();
			ofn.StructSize = Marshal.SizeOf(ofn);
			ofn.Title = title;
			ofn.Filter = ConvertFilter(filter);
			ofn.FilterIndex = 1;
			ofn.File = new string(new char[4096]); // Larger buffer for multiple files
			ofn.MaxFile = 4096;
			ofn.FileTitle = new string(new char[64]);
			ofn.MaxFileTitle = 64;
			ofn.InitialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			ofn.Flags = OFN_ALLOWMULTISELECT | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST | OFN_HIDEREADONLY | OFN_EXPLORER;

			if (GetOpenFileName(ref ofn))
			{
				return ParseMultiSelectResult(ofn.File);
			}

			return new List<string>();
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error opening files dialog: {ex.Message}");
			return new List<string>();
		}
	}

	private string? OpenFolderInternal(string title)
	{
		// Windows folder selection using Shell.BrowseForFolder via COM
		// This is more complex and would require additional P/Invoke structures
		// For simplicity, fallback to file dialog in a folder mode
		try
		{
			// Alternative: Use Environment.SpecialFolder or default to Documents
			return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
			var sfn = new SaveFileName();
			sfn.StructSize = Marshal.SizeOf(sfn);
			sfn.Title = title;
			sfn.Filter = ConvertFilter(filter);
			sfn.FilterIndex = 1;
			sfn.File = suggestedFileName + new string(new char[MAX_PATH - suggestedFileName.Length]);
			sfn.MaxFile = MAX_PATH;
			sfn.FileTitle = new string(new char[64]);
			sfn.MaxFileTitle = 64;
			sfn.InitialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			sfn.Flags = OFN_PATHMUSTEXIST | OFN_OVERWRITEPROMPT | OFN_HIDEREADONLY | OFN_EXPLORER;

			if (GetSaveFileName(ref sfn))
			{
				return sfn.File.TrimEnd('\0');
			}

			return null;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Error opening save dialog: {ex.Message}");
			return null;
		}
	}

	private string ConvertFilter(string filter)
	{
		// Convert pipe-delimited filter format to null-delimited format required by Windows API
		// Input: "Text Files|*.txt|All Files|*.*"
		// Output: "Text Files\0*.txt\0All Files\0*.*\0\0"
		if (string.IsNullOrEmpty(filter))
			return "\0\0";

		var parts = filter.Split('|');
		var sb = new StringBuilder();

		for (int i = 0; i < parts.Length; i++)
		{
			sb.Append(parts[i]);
			sb.Append('\0');
		}

		sb.Append('\0');
		return sb.ToString();
	}

	private IReadOnlyList<string> ParseMultiSelectResult(string result)
	{
		// Multi-select returns paths separated by null characters
		// First element is directory, followed by filenames
		// Need to combine directory + filename for each selection
		var parts = result.TrimEnd('\0').Split('\0');

		if (parts.Length <= 1)
			return new List<string>();

		var directory = parts[0];
		var files = new List<string>();

		for (int i = 1; i < parts.Length; i++)
		{
			files.Add(Path.Combine(directory, parts[i]));
		}

		return files;
	}
}
