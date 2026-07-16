using MUCatalogSharp.Metadata.Parsers;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.Properties;

public record InstallationBehavior(XPathNavigator Navigator, XmlNamespaceManager Manager)
{
    public string? RebootBehavior => _rebootBehavior.Value;
    private readonly Lazy<string?> _rebootBehavior = new(() => 
        UpdateParser.TryCreateOptionalProperty<string>(Navigator, Manager, "@RebootBehavior", out var result) ? result : null);
}
