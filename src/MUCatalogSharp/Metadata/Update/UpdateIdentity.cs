using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update;

public record UpdateIdentity(XPathNavigator Navigator, XmlNamespaceManager Manager) : IIdentity, ICreatable<UpdateIdentity>, IMetadataCollection<UpdateIdentity>
{
    static string[] ICreatable<UpdateIdentity>.ValidParentNodes => ["upd:Update", "upd:SupersededUpdates", "upd:BundledUpdates", "upd:AtLeastOne"];
    static string ICreatable<UpdateIdentity>.XPathQuery => "upd:UpdateIdentity";
    static UpdateIdentity ICreatable<UpdateIdentity>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    public Guid Id => _id.Value;
    private readonly Lazy<Guid> _id = new(() => UpdateParser.CreateProperty<Guid>(Navigator, Manager, "@UpdateID"));

    public int? Revision => _revision.Value;
    private readonly Lazy<int?> _revision = new(() => 
        UpdateParser.TryCreateOptionalProperty<int>(Navigator, Manager, "@RevisionNumber", out var result) ? result : null);

    public int CompareTo(object? obj)
    {
        if (obj is not UpdateIdentity other)
        {
            return -1;
        }

        int idComparison = Id.CompareTo(other.Id);
        if (idComparison != 0)
        {
            return idComparison;
        }

        return Revision?.CompareTo(other?.Revision) ?? 0;
    }

    public override int GetHashCode() => HashCode.Combine(Id, Revision);
}
