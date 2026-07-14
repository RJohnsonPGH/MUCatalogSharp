namespace MUCatalogSharp.Models;

// Detectoids are not actually used, but by keeping track of the ones that have already been processed we can exclude them from future
// metadata downloads, speeding up the syncronization process
public sealed record class Detectoid : Category
{
	public required int? Revision { get; set; }
}
