using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.HandlerSpecificData;

public record CbsData(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<CbsData>, IHandler
{
	public static string[] ValidParentNodes => ["upd:HandlerSpecificData"];
	public static string XPathQuery => "cbs:CbsData";

	static CbsData ICreatable<CbsData>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	public string PackageIdentity => _packageIdentity.Value;
	private readonly Lazy<string> _packageIdentity = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@PackageIdentity"));
}
