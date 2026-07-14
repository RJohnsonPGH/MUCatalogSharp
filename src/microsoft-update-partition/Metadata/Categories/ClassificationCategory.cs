// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Categories;

/// <summary>
/// Represents a classification in the Microsoft Update catalog. 
/// Software or driver updates have a classification: "update", "critical update", etc.
/// </summary>
public sealed class ClassificationCategory : MicrosoftUpdatePackage
{
    internal ClassificationCategory(UpdateIdentity id, XPathNavigator metadataNavigator, XmlNamespaceManager namespaceManager) : 
        base(id, metadataNavigator, namespaceManager) { }
}
