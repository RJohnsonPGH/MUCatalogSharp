// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Categories;

/// <summary>
/// Represents a detectoid in the Microsoft Update catalog. Most software or driver update have one or more corresponding detectoids that
/// check applicability of an update to a device.
/// </summary>
public sealed class DetectoidCategory : MicrosoftUpdatePackage
{
    internal DetectoidCategory(UpdateIdentity id, XPathNavigator metadataNavigator, XmlNamespaceManager namespaceManager) : 
        base(id, metadataNavigator, namespaceManager) { }
}
