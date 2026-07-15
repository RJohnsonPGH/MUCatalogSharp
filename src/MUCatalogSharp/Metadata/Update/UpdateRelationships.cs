using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using MUCatalogSharp.Metadata.Update.Relationships;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update;

public record UpdateRelationships(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<UpdateRelationships>
{
    static string[] ICreatable<UpdateRelationships>.ValidParentNodes => ["upd:Update"];
    static string ICreatable<UpdateRelationships>.XPathQuery => "upd:Relationships";
    static UpdateRelationships ICreatable<UpdateRelationships>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    public UpdatePrerequisites Prerequisites { get; } = UpdateParser.Create<UpdatePrerequisites>(Navigator, Manager);
    public SupersededUpdates? SupersededUpdates { get; } = UpdateParser.CreateOptional<SupersededUpdates>(Navigator, Manager);
	public BundledUpdates? BundledUpdates { get; } = UpdateParser.CreateOptional<BundledUpdates>(Navigator, Manager);
}
