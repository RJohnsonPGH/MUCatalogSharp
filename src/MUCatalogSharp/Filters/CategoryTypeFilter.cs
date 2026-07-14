namespace MUCatalogSharp.Filters;

/// <summary>
/// Represents a filter for category types in the Microsoft Update Catalog.
/// </summary>
[Flags]
public enum CategoryTypeFilter
{
    None = 0,
    Classification = 1 << 0,
    Product = 1 << 1,
    Detectoid = 1 << 2,
    All = Classification | Product | Detectoid
}
