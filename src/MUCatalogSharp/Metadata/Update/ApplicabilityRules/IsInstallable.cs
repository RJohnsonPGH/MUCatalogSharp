using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.ApplicabilityRules;

public record IsInstallable(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<IsInstallable>
{
    static string[] ICreatable<IsInstallable>.ValidParentNodes => ["upd:ApplicabilityRules"];
    static string ICreatable<IsInstallable>.XPathQuery => "upd:IsInstallable";
    static IsInstallable ICreatable<IsInstallable>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);
}