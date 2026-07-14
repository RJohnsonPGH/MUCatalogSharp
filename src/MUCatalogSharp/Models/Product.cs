namespace MUCatalogSharp.Models;

public sealed record Product : Category
{
	public required int? Revision { get; init; }
	public List<Product> Subproducts { get; init; } = [];
	public List<Guid> Categories { get; init; } = [];
}
