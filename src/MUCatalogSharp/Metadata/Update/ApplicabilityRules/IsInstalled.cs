using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.ApplicabilityRules;

public record IsInstalled(XPathNavigator? Navigator, XmlNamespaceManager Manager) : ICreatable<IsInstalled>
{
    static string[] ICreatable<IsInstalled>.ValidParentNodes => ["upd:ApplicabilityRules"];
    static string ICreatable<IsInstalled>.XPathQuery => "upd:IsInstalled";
    static IsInstalled ICreatable<IsInstalled>.Create(XPathNavigator? navigator, XmlNamespaceManager manager) => new(navigator, manager);
}
