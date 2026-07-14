namespace MUCatalogSharp.Models;

public sealed record Update(Guid Id,
	string Title,
	string? Description,
	string? Architecture,
	DateTime CreationDate,
	string? KBArticleId,
	IReadOnlyList<string> Categories,
	IReadOnlyList<Guid> BundledUpdates,
	IReadOnlyList<Guid> SupersededUpdates,
	IReadOnlyList<File> Files) : IUpdate;
