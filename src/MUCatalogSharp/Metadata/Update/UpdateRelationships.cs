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

	public UpdatePrerequisites Prerequisites => _prerequisites.Value;
	private readonly Lazy<UpdatePrerequisites> _prerequisites = new(() =>
		UpdateParser.Create<UpdatePrerequisites>(Navigator, Manager));

	public SupersededUpdates? SupersededUpdates => _supersededUpdates.Value;
	private readonly Lazy<SupersededUpdates?> _supersededUpdates = new(() =>
		UpdateParser.CreateOptional<SupersededUpdates>(Navigator, Manager));

	public BundledUpdates? BundledUpdates => _bundledUpdates.Value;
	private readonly Lazy<BundledUpdates?> _bundledUpdates = new(() =>
		UpdateParser.CreateOptional<BundledUpdates>(Navigator, Manager));
}
