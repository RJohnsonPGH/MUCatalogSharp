// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MUCatalogSharp.Metadata.Update;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Categories;

/// <summary>
/// Represents a product category in the Microsoft Update catalog. 
/// Software or driver updates have one or more corresponding categories: "SQL Server [x]", "Visual Studio [x]", "Windows 1903 and later", etc.
/// </summary>
public sealed class ProductCategory : MicrosoftUpdatePackage
{
    internal ProductCategory(UpdateIdentity id, XPathNavigator metadataNavigator, XmlNamespaceManager namespaceManager) : 
        base(id, metadataNavigator, namespaceManager) { }
}
