using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.Relationships;

public record BundledUpdates(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<BundledUpdates>, IReadOnlyList<UpdateIdentity>
{
	static string[] ICreatable<BundledUpdates>.ValidParentNodes => ["upd:Relationships"];
	static string ICreatable<BundledUpdates>.XPathQuery => "upd:BundledUpdates";
	static BundledUpdates ICreatable<BundledUpdates>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	// Collection interface
	public UpdateIdentity this[int index] => _updateIdentities.Value[index];
	private readonly Lazy<IReadOnlyList<UpdateIdentity>> _updateIdentities = new(() => UpdateParser.CreateCollection<UpdateIdentity>(Navigator, Manager));

	public int Count => _updateIdentities.Value.Count;

	public IEnumerator<UpdateIdentity> GetEnumerator()
	{
		return _updateIdentities.Value.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
