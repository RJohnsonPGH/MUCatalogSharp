using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;
using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.HandlerSpecificData;

public record CbsData(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<CbsData>, IHandler
{
	public static string[] ValidParentNodes => ["upd:HandlerSpecificData"];
	public static string XPathQuery => "cbs:CbsData";

	static CbsData ICreatable<CbsData>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	public string PackageIdentity => _packageIdentity.Value;
	private readonly Lazy<string> _packageIdentity = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@PackageIdentity"));
}
