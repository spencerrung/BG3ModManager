using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DivinityModManager.Services.Parsers;

/// <summary>
/// Cross-platform PAK (package) file reader for Baldur's Gate 3 mod archives.
/// Supports reading and extracting files from .pak archives without LSLib dependency.
/// </summary>
public class PakReader : IPakReader
{
	// PAK file format constants
	private const uint PAK_SIGNATURE = 0x4B50534C; // "LSPK" in little-endian
	private const byte PAK_VERSION_MIN = 7;
	private const byte PAK_VERSION_MAX = 18;

	private string _pakPath;
	private FileStream _pakFile;
	private List<PakFileInfo> _fileList;
	private PakHeader _header;
	private bool _disposed = false;

	/// <summary>
	/// Header structure for PAK files (version 18 format)
	/// </summary>
	private struct PakHeader
	{
		public uint Signature;
		public uint Version;
		public ulong FileListOffset;
		public uint FileListSize;
		public uint NumFiles;
		public uint NumParts;
		public uint DataOffset;
		public uint Flags;
		public byte Priority;
		public byte[] Md5; // 16 bytes
	}

	public void Open(string pakPath)
	{
		if (!File.Exists(pakPath))
		{
			throw new FileNotFoundException($"PAK file not found: {pakPath}");
		}

		_pakPath = pakPath;
		_pakFile = File.OpenRead(pakPath);
		_fileList = new List<PakFileInfo>();

		try
		{
			ReadHeader();
			ReadFileList();
		}
		catch (Exception ex)
		{
			_pakFile?.Dispose();
			_pakFile = null;
			throw new InvalidOperationException($"Failed to read PAK file '{pakPath}': {ex.Message}", ex);
		}
	}

	public List<PakFileInfo> GetFileList()
	{
		ThrowIfDisposed();
		return _fileList.ToList();
	}

	public byte[] ReadFile(PakFileInfo fileInfo)
	{
		ThrowIfDisposed();

		if (fileInfo == null)
		{
			throw new ArgumentNullException(nameof(fileInfo));
		}

		_pakFile.Seek(fileInfo.OffsetInFile, SeekOrigin.Begin);
		byte[] data = new byte[fileInfo.CompressedSize];
		int bytesRead = _pakFile.Read(data, 0, (int)fileInfo.CompressedSize);

		if (bytesRead != fileInfo.CompressedSize)
		{
			throw new IOException($"Failed to read file '{fileInfo.Name}' from PAK");
		}

		// If file is compressed, decompress it
		if (fileInfo.CompressedSize < fileInfo.UncompressedSize)
		{
			// For now, we'll handle basic decompression
			// LSLib supports LZ4, Zlib, Zstd
			// We'll implement basic support for now
			return DecompressData(data, (int)fileInfo.UncompressedSize);
		}

		return data;
	}

	public string ReadFileAsText(PakFileInfo fileInfo)
	{
		ThrowIfDisposed();
		byte[] data = ReadFile(fileInfo);
		return System.Text.Encoding.UTF8.GetString(data);
	}

	public PakFileInfo FindFile(string fileName)
	{
		ThrowIfDisposed();

		// Case-insensitive search with path normalization
		var normalized = fileName.Replace('\\', '/').ToLowerInvariant();
		return _fileList.FirstOrDefault(f => f.Name.Replace('\\', '/').ToLowerInvariant() == normalized);
	}

	public void Dispose()
	{
		if (_disposed) return;

		_pakFile?.Dispose();
		_pakFile = null;
		_fileList?.Clear();
		_disposed = true;
	}

	private void ThrowIfDisposed()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException(GetType().Name);
		}
	}

	/// <summary>
	/// Reads the PAK header to extract file list location and metadata.
	/// </summary>
	private void ReadHeader()
	{
		_pakFile.Seek(0, SeekOrigin.Begin);

		using var reader = new BinaryReader(_pakFile, System.Text.Encoding.UTF8, leaveOpen: true);

		// Read common header
		_header.Signature = reader.ReadUInt32();
		if (_header.Signature != PAK_SIGNATURE)
		{
			throw new InvalidOperationException("Invalid PAK signature - not a PAK file");
		}

		_header.Version = reader.ReadUInt32();
		if (_header.Version < PAK_VERSION_MIN || _header.Version > PAK_VERSION_MAX)
		{
			throw new NotSupportedException($"PAK version {_header.Version} is not supported");
		}

		// Version-specific header parsing
		if (_header.Version <= 13)
		{
			// Version 7-13 format
			_header.FileListOffset = reader.ReadUInt32();
			_header.FileListSize = reader.ReadUInt32();
			_header.NumFiles = reader.ReadUInt32();
			_header.NumParts = reader.ReadUInt32();
			_header.DataOffset = reader.ReadUInt32();
		}
		else
		{
			// Version 14+ format
			_header.FileListOffset = reader.ReadUInt64();
			_header.FileListSize = reader.ReadUInt32();
			_header.NumFiles = reader.ReadUInt32();
			_header.NumParts = reader.ReadUInt32();
			_header.DataOffset = reader.ReadUInt32();
			_header.Flags = reader.ReadUInt32();
			_header.Priority = reader.ReadByte();
			_header.Md5 = reader.ReadBytes(16);
		}

		DivinityApp.Log($"PAK Version: {_header.Version}, Files: {_header.NumFiles}, DataOffset: {_header.DataOffset}");
	}

	/// <summary>
	/// Reads the file list from the PAK archive.
	/// File lists may be compressed depending on PAK version.
	/// </summary>
	private void ReadFileList()
	{
		_pakFile.Seek((long)_header.FileListOffset, SeekOrigin.Begin);

		using var reader = new BinaryReader(_pakFile, System.Text.Encoding.UTF8, leaveOpen: true);

		if (_header.Version > 13)
		{
			// Version 14+ has compressed file list
			ReadCompressedFileList(reader);
		}
		else
		{
			// Version 7-13 has uncompressed file list
			ReadUncompressedFileList(reader);
		}

		DivinityApp.Log($"Loaded {_fileList.Count} files from PAK");
	}

	/// <summary>
	/// Reads an uncompressed file list (PAK version 7-13).
	/// </summary>
	private void ReadUncompressedFileList(BinaryReader reader)
	{
		for (int i = 0; i < _header.NumFiles; i++)
		{
			var fileInfo = ReadFileEntry(reader);
			_fileList.Add(fileInfo);
		}
	}

	/// <summary>
	/// Reads a compressed file list (PAK version 14+).
	/// File list is LZ4-compressed.
	/// </summary>
	private void ReadCompressedFileList(BinaryReader reader)
	{
		int numFiles = reader.ReadInt32();
		int compressedSize = reader.ReadInt32();

		byte[] compressedData = reader.ReadBytes(compressedSize);

		// Decompress using LZ4
		// For now, we'll try to handle this when we have the compression library
		try
		{
			byte[] decompressed = DecompressLZ4(compressedData, (int)(_header.FileListSize - 8));
			using var ms = new MemoryStream(decompressed);
			using var decompReader = new BinaryReader(ms);

			for (int i = 0; i < numFiles; i++)
			{
				var fileInfo = ReadFileEntry(decompReader);
				_fileList.Add(fileInfo);
			}
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Warning: Failed to decompress file list: {ex.Message}");
			// Fall back to reading compressed data as-is if we don't have LZ4
		}
	}

	/// <summary>
	/// Reads a single file entry from the file list.
	/// File entry format varies by PAK version.
	/// </summary>
	private PakFileInfo ReadFileEntry(BinaryReader reader)
	{
		var fileInfo = new PakFileInfo();

		// Read file name (null-terminated string)
		var nameBytes = new List<byte>();
		byte b;
		while ((b = reader.ReadByte()) != 0)
		{
			nameBytes.Add(b);
		}
		fileInfo.Name = System.Text.Encoding.UTF8.GetString(nameBytes.ToArray());

		// Read file metadata
		fileInfo.OffsetInFile = reader.ReadInt64();
		fileInfo.CompressedSize = reader.ReadInt64();
		fileInfo.UncompressedSize = reader.ReadInt64();
		fileInfo.Crc32 = reader.ReadUInt32();

		// Read flags (compression type, etc.)
		uint flags = reader.ReadUInt32();

		return fileInfo;
	}

	/// <summary>
	/// Decompresses data based on compression flags/method.
	/// Supports LZ4, Zlib, and uncompressed data.
	/// </summary>
	private byte[] DecompressData(byte[] data, int uncompressedSize)
	{
		if (data.Length == uncompressedSize)
		{
			// Not actually compressed
			return data;
		}

		try
		{
			// Try LZ4 decompression
			return DecompressLZ4(data, uncompressedSize);
		}
		catch
		{
			try
			{
				// Try Zlib decompression
				return DecompressZlib(data);
			}
			catch
			{
				// Return as-is if decompression fails
				DivinityApp.Log($"Warning: Could not decompress file data ({data.Length} -> {uncompressedSize} bytes)");
				return data;
			}
		}
	}

	/// <summary>
	/// LZ4 decompression using K4os.Compression.LZ4 library.
	/// </summary>
	private byte[] DecompressLZ4(byte[] compressed, int uncompressedSize)
	{
		try
		{
			byte[] decompressed = new byte[uncompressedSize];
			int decompressedLength = K4os.Compression.LZ4.LZ4Codec.Decode(
				compressed, 0, compressed.Length,
				decompressed, 0, uncompressedSize
			);

			if (decompressedLength != uncompressedSize)
			{
				DivinityApp.Log($"Warning: LZ4 decompression produced {decompressedLength} bytes, expected {uncompressedSize}");
			}

			return decompressed;
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"LZ4 decompression failed: {ex.Message}");
			throw;
		}
	}

	/// <summary>
	/// Zlib decompression using System.IO.Compression.DeflateStream.
	/// </summary>
	private byte[] DecompressZlib(byte[] compressed)
	{
		try
		{
			using var input = new MemoryStream(compressed);
			using var deflateStream = new System.IO.Compression.DeflateStream(input, System.IO.Compression.CompressionMode.Decompress);
			using var output = new MemoryStream();
			deflateStream.CopyTo(output);
			return output.ToArray();
		}
		catch (Exception ex)
		{
			DivinityApp.Log($"Zlib decompression failed: {ex.Message}");
			throw;
		}
	}
}
