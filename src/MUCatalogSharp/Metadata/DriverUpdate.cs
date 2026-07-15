using MUCatalogSharp.Metadata.Content;
using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Update;
using MUCatalogSharp.Metadata.Update.ApplicabilityRules.Metadata;
using MUCatalogSharp.Metadata.Update.Files;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata;

public sealed class DriverUpdate(UpdateIdentity id, XPathNavigator metadataNavigator, XmlNamespaceManager namespaceManager, IReadOnlyDictionary<string, UpdateFileUri> fileUris) : 
    MicrosoftUpdatePackage(id, metadataNavigator, namespaceManager)
{
	public IReadOnlyCollection<UpdateFile>? ResolvedFiles => _resolvedFiles.Value;
	private readonly Lazy<IReadOnlyCollection<UpdateFile>?> _resolvedFiles = new(() =>
		UpdateParser.ResolveFiles(metadataNavigator, namespaceManager, fileUris));

	public WindowsDriverMetadata DriverMetadata => _driverMetadata.Value;
    private readonly Lazy<WindowsDriverMetadata> _driverMetadata =
        new(() => UpdateParser.Create<WindowsDriverMetadata>(metadataNavigator, namespaceManager));
}
