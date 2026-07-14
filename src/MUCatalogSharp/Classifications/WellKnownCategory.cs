namespace MUCatalogSharp.Classifications;

/// <summary>
/// Represents a well-known product in the Microsoft Update Catalog.
/// </summary>
public record WellKnownProduct : WellKnownCategory
{
	public static readonly WellKnownProduct Windows10 = new(new Guid("8a3cbc4a-5334-40d4-a06e-6da96022ae3b"));
	public static readonly WellKnownProduct Windows11 = new(new Guid("72e7624a-5b00-45d2-b92f-e561c0a6a160"));
	public static readonly WellKnownProduct WindowsServer2019 = new(new Guid("f702a48c-919b-45d6-9aef-ca4248d50397")); //6e56e6da-f22f-47c9-97b4-510153a06740
	public static readonly WellKnownProduct WindowsServer2022 = new(new Guid("2c7888b6-f9e9-4ee9-87af-a77705193893"));
	public static readonly WellKnownProduct WindowsServer2025 = new(new Guid("b256987d-4693-4c87-955d-dbb9341205eb"));

	// Product Drivers
	public static readonly WellKnownProduct Windows10Drivers = new(new Guid("05eebf61-148b-43cf-80da-1c99ab0b8699"));
	public static readonly WellKnownProduct Windows11Drivers = new(new Guid("bfeb1830-4b34-40d0-a2d7-8d6f994ddb58"));

	private WellKnownProduct(Guid guid)
		: base(guid) { }
}

/// <summary>
/// Represents a well-known classification in the Microsoft Update Catalog.
/// </summary>
public record WellKnownClassification : WellKnownCategory
{
	public static readonly WellKnownClassification Tools = new(new Guid("b4832bd8-e735-4761-8daf-37f882276dab"));
	public static readonly WellKnownClassification ServicePacks = new(new Guid("68c5b0a3-d1a6-4553-ae49-01d3a7827828"));
	public static readonly WellKnownClassification UpdateRollups = new(new Guid("28bc880e-0592-4cbf-8f95-c79b17911d5f"));
	public static readonly WellKnownClassification FeaturePacks = new(new Guid("b54e7d24-7add-428f-8b75-90a396fa584f"));
	public static readonly WellKnownClassification CriticalUpdates = new(new Guid("e6cf1350-c01b-414d-a61f-263d14d133b4"));
	public static readonly WellKnownClassification Updates = new(new Guid("cd5ffd1e-e932-4e3a-bf74-18bf0b1bbd83"));
	public static readonly WellKnownClassification Upgrades = new(new Guid("3689bdc8-b205-4af4-8d4a-a63924c5e9d5"));
	public static readonly WellKnownClassification DefinitionUpdates = new(new Guid("e0789628-ce08-4437-be74-2495b842f43b"));
	public static readonly WellKnownClassification SecurityUpdates = new(new Guid("0fa1201d-4330-4fa8-8ae9-b877473b6441"));
	public static readonly WellKnownClassification Drivers = new(new Guid("ebfc1fc5-71a4-4f7b-9aca-3b9a503104a0"));

	private WellKnownClassification(Guid guid)
		: base(guid) { }
}

/// <summary>
/// Represents a well-known category (either a product or classification) in the Microsoft Update Catalog.
/// </summary>
public record WellKnownCategory
{
	public Guid Id { get; }
	internal WellKnownCategory(Guid guid)
	{ 
		Id = guid;
	}

	public static implicit operator Guid(WellKnownCategory category) => category.Id;
}
