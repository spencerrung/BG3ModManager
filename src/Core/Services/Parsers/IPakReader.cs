using System;
using System.Collections.Generic;

namespace DivinityModManager.Services.Parsers;

/// <summary>
/// Information about a file contained in a PAK archive.
/// </summary>
public class PakFileInfo
{
	/// <summary>
	/// Name of the file (may include path separators)
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Size of the file when uncompressed
	/// </summary>
	public long UncompressedSize { get; set; }

	/// <summary>
	/// Size of the file when compressed in the PAK
	/// </summary>
	public long CompressedSize { get; set; }

	/// <summary>
	/// CRC32 checksum of the file
	/// </summary>
	public uint Crc32 { get; set; }

	/// <summary>
	/// Offset in the archive where this file's data starts
	/// </summary>
	public long OffsetInFile { get; set; }
}

/// <summary>
/// Interface for reading PAK (package) files used in Baldur's Gate 3.
/// PAK files are binary archive formats that contain game assets and mod packages.
/// </summary>
public interface IPakReader : IDisposable
{

	/// <summary>
	/// Opens a PAK file for reading.
	/// </summary>
	/// <param name="pakPath">Full path to the .pak file</param>
	void Open(string pakPath);

	/// <summary>
	/// Gets the list of all files contained in the PAK archive.
	/// </summary>
	/// <returns>List of PakFileInfo objects describing all files in the PAK</returns>
	List<PakFileInfo> GetFileList();

	/// <summary>
	/// Extracts and reads the contents of a file from the PAK archive.
	/// </summary>
	/// <param name="fileInfo">The file to read (from GetFileList)</param>
	/// <returns>The decompressed file contents as a byte array</returns>
	byte[] ReadFile(PakFileInfo fileInfo);

	/// <summary>
	/// Extracts and reads a file as text (UTF-8).
	/// Convenience method for text-based files like XML.
	/// </summary>
	/// <param name="fileInfo">The file to read</param>
	/// <returns>The file contents as a UTF-8 string</returns>
	string ReadFileAsText(PakFileInfo fileInfo);

	/// <summary>
	/// Finds a file in the PAK by name (case-insensitive).
	/// </summary>
	/// <param name="fileName">The file name to find (may include path)</param>
	/// <returns>PakFileInfo if found, null otherwise</returns>
	PakFileInfo FindFile(string fileName);
}
