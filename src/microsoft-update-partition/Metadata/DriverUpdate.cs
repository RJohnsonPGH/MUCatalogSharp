using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Content;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.ApplicabilityRules.Metadata;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.Files;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata;

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
