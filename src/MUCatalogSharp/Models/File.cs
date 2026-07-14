namespace MUCatalogSharp.Models;

public sealed record File
{
	public required string FileName { get; init; }
	public required Uri DownloadUri { get; init; }
	public required DateTime ModifiedDate { get; init; }
	public required string Sha1Hash { get; init; }
	public required ulong Size { get; init; }
}
