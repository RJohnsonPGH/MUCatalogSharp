using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.Files;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;

public record UpdateFilesCollection(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<UpdateFilesCollection>, IReadOnlyList<UnresolvedUpdateFile>
{
	// Creatable interface
	static string[] ICreatable<UpdateFilesCollection>.ValidParentNodes => ["upd:Update"];
	static string ICreatable<UpdateFilesCollection>.XPathQuery => "upd:Files";
	static UpdateFilesCollection ICreatable<UpdateFilesCollection>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	// Collection interface
	public UnresolvedUpdateFile this[int index] => _updateFiles.Value[index];
	private readonly Lazy<IReadOnlyList<UnresolvedUpdateFile>> _updateFiles = new(() => 
		UpdateParser.CreateCollection<UnresolvedUpdateFile>(Navigator, Manager));

	public int Count => _updateFiles.Value.Count;

	public IEnumerator<UnresolvedUpdateFile> GetEnumerator()
	{
		return _updateFiles.Value.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
