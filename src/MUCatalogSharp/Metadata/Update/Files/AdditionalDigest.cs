using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.Files;

public record AdditionalDigest(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<AdditionalDigest>, IMetadataCollection<AdditionalDigest>
{
    // ICreatable
    static string[] ICreatable<AdditionalDigest>.ValidParentNodes => ["upd:File"];
    static string ICreatable<AdditionalDigest>.XPathQuery => "upd:AdditionalDigest";
    static AdditionalDigest ICreatable<AdditionalDigest>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    // Properties
    public string Algorithm => _algorithm.Value;
    public Lazy<string> _algorithm = new(() => UpdateParser.CreateProperty<string>(Navigator, Manager, "@Algorithm"));

    public string Digest => _digest.Value;
    public Lazy<string> _digest = new(() => UpdateParser.CreateProperty<string>(Navigator, Manager, "text()"));
}
