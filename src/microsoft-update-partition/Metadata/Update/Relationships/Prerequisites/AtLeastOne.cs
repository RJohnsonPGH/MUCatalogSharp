using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.Relationships.Prerequisites;

public record AtLeastOne(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<AtLeastOne>, IMetadataCollection<AtLeastOne>
{
	static string[] ICreatable<AtLeastOne>.ValidParentNodes => ["upd:Prerequisites"];
	static string ICreatable<AtLeastOne>.XPathQuery => "upd:AtLeastOne";
	static AtLeastOne ICreatable<AtLeastOne>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	public IReadOnlyList<UpdateIdentity> UpdateIdentities => _updateIdentities.Value;
	private readonly Lazy<IReadOnlyList<UpdateIdentity>> _updateIdentities = new(() => 
		UpdateParser.CreateCollection<UpdateIdentity>(Navigator, Manager));

	public bool IsCategory => _isCategory.Value;
	private readonly  Lazy<bool> _isCategory = new(() =>
		UpdateParser.TryCreateOptionalProperty<bool>(Navigator, Manager, "@IsCategory", out var value) && value);
}