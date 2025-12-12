using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DivinityModManager.Services.Parsers;

/// <summary>
/// Cross-platform implementation of LSX parser.
/// Works on both Windows and Linux using only .NET standard libraries.
/// </summary>
public class LsxParser : ILsxParser
{
	// LSX attribute escape pattern - handles invalid XML attribute names
	// Pattern: attribute id="AttributeName" value="AttributeValue" type="AttributeType"
	private static readonly Regex _xmlAttributeFixRegex = new(
		@"(?<!\\)""(?<!\\)""",
		RegexOptions.Compiled
	);

	public XElement ParseXml(string xmlContent)
	{
		if (string.IsNullOrWhiteSpace(xmlContent))
		{
			throw new ArgumentException("XML content cannot be null or empty", nameof(xmlContent));
		}

		try
		{
			// LSX files may have escaped quotes in attributes that need fixing
			var fixedContent = EscapeXmlAttributes(xmlContent);
			return XElement.Parse(fixedContent);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to parse LSX content: {ex.Message}", ex);
		}
	}

	public string GetAttributeValueWithId(XElement node, string attributeId, string defaultValue = "")
	{
		if (node == null) return defaultValue;

		var attribute = GetAttributeWithId(node, attributeId);
		if (attribute == null) return defaultValue;

		var valueAttr = attribute.Attribute("value");
		return valueAttr?.Value ?? defaultValue;
	}

	public XElement GetAttributeWithId(XElement node, string attributeId)
	{
		if (node == null) return null;

		return node.Descendants("attribute")
			.FirstOrDefault(a => a.Attribute("id")?.Value == attributeId);
	}

	public string UnescapeXml(string text)
	{
		if (string.IsNullOrEmpty(text)) return text;

		// XDocument handles standard XML entity unescaping automatically
		// But we need to handle some edge cases that LSX uses
		return text
			.Replace("&lt;", "<")
			.Replace("&gt;", ">")
			.Replace("&amp;", "&")
			.Replace("&quot;", "\"")
			.Replace("&apos;", "'");
	}

	/// <summary>
	/// Escapes problematic XML attribute patterns in LSX content.
	/// LSX files sometimes contain attribute values with special characters that need escaping.
	/// </summary>
	private static string EscapeXmlAttributes(string content)
	{
		if (string.IsNullOrEmpty(content)) return content;

		// Fix malformed empty string attributes: "" -> ""
		// This prevents XML parsing errors
		return _xmlAttributeFixRegex.Replace(content, "&#34;&#34;");
	}
}
