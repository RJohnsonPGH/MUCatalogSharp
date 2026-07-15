using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update;

public record LocalizedPropertiesCollection(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<LocalizedPropertiesCollection>, IReadOnlyList<LocalizedProperties>
{
    // Creatable interface
    static string[] ICreatable<LocalizedPropertiesCollection>.ValidParentNodes => ["upd:Update"];
    static string ICreatable<LocalizedPropertiesCollection>.XPathQuery => "upd:LocalizedPropertiesCollection";
    static LocalizedPropertiesCollection ICreatable<LocalizedPropertiesCollection>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    // Collection interface
    public LocalizedProperties this[int index] => _localizedProperties.Value[index];
    private readonly Lazy<IReadOnlyList<LocalizedProperties>> _localizedProperties = new(() => UpdateParser.CreateCollection<LocalizedProperties>(Navigator, Manager));

    public int Count => _localizedProperties.Value.Count;

    public IEnumerator<LocalizedProperties> GetEnumerator()
    {
        return _localizedProperties.Value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public record LocalizedProperties(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<LocalizedProperties>, IMetadataCollection<LocalizedProperties>
{
    // ICreatable
    static string[] ICreatable<LocalizedProperties>.ValidParentNodes => throw new NotImplementedException();
    static string ICreatable<LocalizedProperties>.XPathQuery => "upd:LocalizedProperties";
    static LocalizedProperties ICreatable<LocalizedProperties>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    // Properties
    public string Language => _language.Value;
    private readonly Lazy<string> _language = new(() => UpdateParser.CreateProperty<string>(Navigator, Manager, "upd:Language"));

    public string Title => _title.Value;
    private readonly Lazy<string> _title = new(() => UpdateParser.CreateProperty<string>(Navigator, Manager, "upd:Title"));

    public string? Description => _description.Value;
    private readonly Lazy<string?> _description = new(() => 
        UpdateParser.TryCreateOptionalProperty<string>(Navigator, Manager, "upd:Description", out var result) ? result : null);

    public string? UninstallNotes => _uninstallNotes.Value;
	private readonly Lazy<string?> _uninstallNotes = new(() =>
		UpdateParser.TryCreateOptionalProperty<string>(Navigator, Manager, "upd:UninstallNotes", out var result) ? result : null);

    public ParsableUri? MoreInfoUrl => _moreInfoUrl.Value;
    private readonly Lazy<ParsableUri?> _moreInfoUrl = new(() =>
		UpdateParser.TryCreateOptionalProperty<ParsableUri>(Navigator, Manager, "upd:MoreInfoURL", out var result) ? result : null);

    public ParsableUri? SupportUrl => _supportUrl.Value;
    private readonly Lazy<ParsableUri?> _supportUrl = new(() =>
	    UpdateParser.TryCreateOptionalProperty<ParsableUri>(Navigator, Manager, "upd:SupportURL", out var result) ? result : null);
}
