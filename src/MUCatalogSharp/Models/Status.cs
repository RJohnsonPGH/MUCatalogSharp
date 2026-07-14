using System.Text.Json.Serialization;

namespace MUCatalogSharp.Models;

public sealed record Status
{
	public static readonly string LoadingMetadata = "Loading Metadata";
	public static readonly string Idle = "Idle";

	[JsonIgnore]
	public required string Id { get; set; }
	public required bool InitialSyncComplete { get; set; }
	public required string State { get; set; }
}
