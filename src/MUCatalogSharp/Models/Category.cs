namespace MUCatalogSharp.Models;

public abstract record Category
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
