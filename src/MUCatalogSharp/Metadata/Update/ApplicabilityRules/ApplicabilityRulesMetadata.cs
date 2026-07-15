using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using MUCatalogSharp.Metadata.Update.ApplicabilityRules.Metadata;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.ApplicabilityRules;

public record ApplicabilityRulesMetadata(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<ApplicabilityRulesMetadata>
{
    static string[] ICreatable<ApplicabilityRulesMetadata>.ValidParentNodes => ["upd:ApplicabilityRules"];
    static string ICreatable<ApplicabilityRulesMetadata>.XPathQuery => "upd:Metadata";
    static ApplicabilityRulesMetadata ICreatable<ApplicabilityRulesMetadata>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    public PackageApplicabilityMetadata? PackageApplicabilityMetadata => _packageApplicabilityMetadata.Value;
    private readonly Lazy<PackageApplicabilityMetadata?> _packageApplicabilityMetadata = new(() =>
        UpdateParser.CreateOptional<PackageApplicabilityMetadata>(Navigator, Manager));
	
    public WindowsDriverMetadata? WindowsDriverMetadata => _windowsDriverMetadata.Value;
	private readonly Lazy<WindowsDriverMetadata?> _windowsDriverMetadata = new(() =>
		UpdateParser.CreateOptional<WindowsDriverMetadata>(Navigator, Manager));
}
