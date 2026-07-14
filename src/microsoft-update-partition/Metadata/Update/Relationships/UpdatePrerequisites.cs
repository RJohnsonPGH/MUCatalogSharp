using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.Relationships.Prerequisites;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.Relationships;

public record UpdatePrerequisites(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<UpdatePrerequisites>, IReadOnlyList<UpdateIdentity>
{
    // ICreatable
    static string[] ICreatable<UpdatePrerequisites>.ValidParentNodes => ["upd:Relationships"];
    static string ICreatable<UpdatePrerequisites>.XPathQuery => "upd:Prerequisites";
    static UpdatePrerequisites ICreatable<UpdatePrerequisites>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

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

	public IReadOnlyList<AtLeastOne> AtLeastOnePrerequisites => _atLeastOnePrerequisites.Value;
	private readonly Lazy<IReadOnlyList<AtLeastOne>> _atLeastOnePrerequisites = new(() => 
		UpdateParser.CreateCollection<AtLeastOne>(Navigator, Manager));
}
