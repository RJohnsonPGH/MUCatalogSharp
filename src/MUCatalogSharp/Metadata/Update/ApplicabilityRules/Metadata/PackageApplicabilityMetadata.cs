using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using MUCatalogSharp.Metadata.Update.ApplicabilityRules.Metadata.CbsPackageApplicabilityMetadata;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.ApplicabilityRules.Metadata;

public record PackageApplicabilityMetadata(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<PackageApplicabilityMetadata>
{
    static string[] ICreatable<PackageApplicabilityMetadata>.ValidParentNodes => ["upd:Metadata"];
    static string ICreatable<PackageApplicabilityMetadata>.XPathQuery => "cbsar:CbsPackageApplicabilityMetadata";
    static PackageApplicabilityMetadata ICreatable<PackageApplicabilityMetadata>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    public IReadOnlyList<Assembly> Assemblies => _assemblies.Value;
    private readonly Lazy<IReadOnlyList<Assembly>> _assemblies = new(() => 
        UpdateParser.CreateCollection<Assembly>(Navigator, Manager));
}
