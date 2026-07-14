namespace MUCatalogSharp.Models;

public sealed record Driver(Guid Id,
	string Title,
	string? Description,
	DateTime CreationDate,
	IReadOnlyList<string> Categories,
	IReadOnlyList<File> Files) : IUpdate;
