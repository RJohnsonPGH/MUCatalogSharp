using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.HandlerSpecificData;

public record CategoryInformation(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<CategoryInformation>, IHandler
{
	public static string[] ValidParentNodes => ["upd:HandlerSpecificData"];
    public static string XPathQuery => "cat:CategoryInformation";

	static CategoryInformation ICreatable<CategoryInformation>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	public enum CategoryType
    {
        UpdateClassification,
        Product,
        Company,
        ProductFamily
    }

    public CategoryType Type => _categoryType.Value;
    private readonly Lazy<CategoryType> _categoryType =
        new(() => UpdateParser.CreateProperty<ParsableEnum<CategoryType>>(Navigator, Manager, "@CategoryType"));

	public bool ProhibitsSubcategories => _prohibitsSubcategories.Value;
	private readonly Lazy<bool> _prohibitsSubcategories =
		new(() => UpdateParser.CreateProperty<bool>(Navigator, Manager, "@ProhibitsSubcategories"));


	public bool? ProhibitsUpdates => _prohibitsUpdates.Value;
	private readonly Lazy<bool?> _prohibitsUpdates =
		new(() => UpdateParser.TryCreateOptionalProperty<bool>(Navigator, Manager, "@ProhibitsUpdates", out var value) ? value : null);

	public int? DisplayOrder => _displayOrder.Value;
	private readonly Lazy<int?> _displayOrder =
		new(() => UpdateParser.TryCreateOptionalProperty<int>(Navigator, Manager, "@DisplayOrder", out var value) ? value : null);

	public bool? ExcludedByDefault => _excludedByDefault.Value;
	private readonly Lazy<bool?> _excludedByDefault =
		new(() => UpdateParser.TryCreateOptionalProperty<bool>(Navigator, Manager, "@ExcludedByDefault", out var value) ? value : null);
}
