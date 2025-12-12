using System;
using System.Xml.Linq;

namespace DivinityModManager.Services.Parsers;

/// <summary>
/// Cross-platform parser for LSX (Larian Studio XML) files.
/// LSX is a standardized XML format used for Baldur's Gate 3 mod metadata and configuration.
/// </summary>
public interface ILsxParser
{
	/// <summary>
	/// Parses an LSX (XML) string and returns the root XElement.
	/// Handles LSX-specific XML escaping and normalization.
	/// </summary>
	/// <param name="xmlContent">Raw XML content as string</param>
	/// <returns>Parsed XElement representing the root node</returns>
	XElement ParseXml(string xmlContent);

	/// <summary>
	/// Gets an attribute value from a node by attribute ID.
	/// LSX files store attributes in a child &lt;attribute&gt; nodes with id attributes.
	/// </summary>
	/// <param name="node">The parent XElement node</param>
	/// <param name="attributeId">The id of the attribute to find</param>
	/// <param name="defaultValue">Value to return if attribute not found</param>
	/// <returns>The attribute value or default value</returns>
	string GetAttributeValueWithId(XElement node, string attributeId, string defaultValue = "");

	/// <summary>
	/// Gets an attribute element from a node by attribute ID.
	/// </summary>
	/// <param name="node">The parent XElement node</param>
	/// <param name="attributeId">The id of the attribute to find</param>
	/// <returns>The attribute XElement or null</returns>
	XElement GetAttributeWithId(XElement node, string attributeId);

	/// <summary>
	/// Unescapes XML-escaped strings in LSX content.
	/// LSX uses standard XML entity escaping (&lt;, &gt;, &amp;, etc.)
	/// </summary>
	/// <param name="text">The escaped text</param>
	/// <returns>The unescaped text</returns>
	string UnescapeXml(string text);
}
