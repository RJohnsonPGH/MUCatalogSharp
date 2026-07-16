using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.Properties;

public record Language(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<Language>, IMetadataCollection<Language>
{
	static string[] ICreatable<Language>.ValidParentNodes => ["upd:Properties"];
	static string ICreatable<Language>.XPathQuery => "upd:Language";
	static Language ICreatable<Language>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	public string Value => _value.Value;
	private readonly Lazy<string> _value = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "text()"));

	public static implicit operator string(Language language) => language.Value;
}
