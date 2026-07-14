using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Categories;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Content;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.HandlerSpecificData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata;

/// <summary>
/// Represents a Microsoft Update package with its associated metadata.
/// </summary>
/// <param name="id">The identity of the update.</param>
/// <param name="metadataNavigator">The XPathNavigator positioned at the XML node to parse.</param>
/// <param name="namespaceManager">The XmlNamespaceManager for resolving namespaces in the XML document.</param>
public abstract class MicrosoftUpdatePackage(UpdateIdentity id, XPathNavigator metadataNavigator, XmlNamespaceManager namespaceManager)
{
    public UpdateIdentity Id => id;

	public UpdateProperties UpdateProperties => _updateProperties.Value;
	private readonly Lazy<UpdateProperties> _updateProperties = new(() =>
        UpdateParser.Create<UpdateProperties>(metadataNavigator, namespaceManager));

	public LocalizedPropertiesCollection LocalizedProperties => _localizedProperties.Value;
    private readonly Lazy<LocalizedPropertiesCollection> _localizedProperties = new(() => 
        UpdateParser.Create<LocalizedPropertiesCollection>(metadataNavigator, namespaceManager));

    public UpdateRelationships Relationships => _relationships.Value;
    private readonly Lazy<UpdateRelationships> _relationships = new(() => 
        UpdateParser.Create<UpdateRelationships>(metadataNavigator, namespaceManager));

    public UpdateApplicabilityRules? ApplicabilityRules => _applicabilityRules.Value;
    private readonly Lazy<UpdateApplicabilityRules?> _applicabilityRules = new(() => 
        UpdateParser.CreateOptional<UpdateApplicabilityRules>(metadataNavigator, namespaceManager));

	/// <summary>
	/// Creates a MicrosoftUpdatePackage instance from the provided metadata XML stream and file URIs.
	/// </summary>
	/// <param name="metadataStream">The stream containing the metadata XML.</param>
	/// <param name="fileUris">A dictionary mapping file digests to their corresponding URIs.</param>
	/// <returns>A MicrosoftUpdatePackage instance.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the metadata XML is invalid or cannot be parsed.</exception>
	/// <exception cref="NotSupportedException">Thrown if the update type is not supported.</exception>
	public static MicrosoftUpdatePackage FromMetadataXml(Stream metadataStream, Dictionary<string, UpdateFileUri> fileUris)
    {
        XPathDocument document = new(metadataStream);
        XPathNavigator navigator = document.CreateNavigator();

        XmlNamespaceManager manager = new(navigator.NameTable);
        manager.AddNamespace("upd", "http://schemas.microsoft.com/msus/2002/12/Update");
        manager.AddNamespace("cat", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/Category");
        manager.AddNamespace("drv", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/WindowsDriver");
        manager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        manager.AddNamespace("cmd", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/CommandLineInstallation");
        manager.AddNamespace("psf", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/WindowsPatch");
        manager.AddNamespace("cbs", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/Cbs");
        manager.AddNamespace("cbsar", "http://schemas.microsoft.com/msus/2002/12/CbsApplicabilityRules");
        manager.AddNamespace("msp", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/WindowsInstaller");
        manager.AddNamespace("wsi", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/WindowsSetup");
		manager.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v3");

		// Move the navigator to the upd:Update root node
		var rootNavigator = navigator.SelectSingleNode("upd:Update", manager)
            ?? throw new InvalidOperationException("Failed to select the node 'upd:Update'");

        var id = UpdateParser.Create<UpdateIdentity>(rootNavigator, manager);
        var updateProperties = UpdateParser.Create<UpdateProperties>(rootNavigator, manager);

		return updateProperties.Type switch
        {
			UpdateProperties.UpdateType.Detectoid => new DetectoidCategory(id, rootNavigator, manager),
			UpdateProperties.UpdateType.Category => GetCategoryTypeFromUpdateProperties(rootNavigator, manager) switch
            {
				CategoryInformation.CategoryType.UpdateClassification => new ClassificationCategory(id, rootNavigator, manager),
                CategoryInformation.CategoryType.Product or
                CategoryInformation.CategoryType.ProductFamily or
                CategoryInformation.CategoryType.Company => new ProductCategory(id, rootNavigator, manager),
				_ => throw new NotSupportedException($"Category type {updateProperties.Type} is not supported"),
			},
			UpdateProperties.UpdateType.Driver => new DriverUpdate(id, rootNavigator, manager, fileUris),
			UpdateProperties.UpdateType.Software => new SoftwareUpdate(id, rootNavigator, manager, fileUris),
			_ => throw new NotSupportedException($"Update type {updateProperties.Type} is not supported"),
		};
    }

	/// <summary>
	/// Gets the category type from the update properties of the given navigator.
	/// </summary>
	/// <param name="navigator">The XPathNavigator positioned at the XML node to parse.</param>
	/// <param name="manager">The XmlNamespaceManager for resolving namespaces in the XML document.</param>
	/// <returns>The category type from the update properties.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the handler specific data is not of type CategoryInformation.</exception>
	private static CategoryInformation.CategoryType GetCategoryTypeFromUpdateProperties(XPathNavigator navigator, XmlNamespaceManager manager)
	{
        var handlerSpecificData = UpdateParser.Create<UpdateHandlerSpecificData>(navigator, manager);

        if (handlerSpecificData.Handler is not CategoryInformation categoryInformation)
        {
			throw new InvalidOperationException($"Handler specific data is not of type CategoryInformation.");
		}

        return categoryInformation.Type;
	}
}
