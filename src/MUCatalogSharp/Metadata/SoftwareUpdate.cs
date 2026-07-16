using MUCatalogSharp.Metadata.Content;
using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Update;
using MUCatalogSharp.Metadata.Update.Files;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata;

public class SoftwareUpdate(
	UpdateIdentity id, 
	XPathNavigator metadataNavigator, 
	XmlNamespaceManager namespaceManager, 
	IReadOnlyDictionary<string, UpdateFileUri> fileUris) :
    MicrosoftUpdatePackage(id, metadataNavigator, namespaceManager)
{
	public IReadOnlyCollection<UpdateFile>? ResolvedFiles => _resolvedFiles.Value;
	private readonly Lazy<IReadOnlyCollection<UpdateFile>?> _resolvedFiles = new(() => 
		UpdateParser.ResolveFiles(metadataNavigator, namespaceManager, fileUris));
}
