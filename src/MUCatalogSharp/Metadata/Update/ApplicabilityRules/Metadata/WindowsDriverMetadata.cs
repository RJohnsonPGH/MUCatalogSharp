using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using MUCatalogSharp.Metadata.Update.ApplicabilityRules.Metadata.Drivers;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.ApplicabilityRules.Metadata;

public sealed record WindowsDriverMetadata(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<WindowsDriverMetadata>
{
	static string[] ICreatable<WindowsDriverMetadata>.ValidParentNodes => ["upd:Metadata"];
	static string ICreatable<WindowsDriverMetadata>.XPathQuery => "drv:WindowsDriverMetaData";
	static WindowsDriverMetadata ICreatable<WindowsDriverMetadata>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    public string HardwareId => _hardwareId.Value;
	private readonly Lazy<string> _hardwareId = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@HardwareID"));

	public string WhqlDriverID => _whqlDriverID.Value;
	private readonly Lazy<string> _whqlDriverID = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@WhqlDriverID"));

    public string Manufacturer => _manufacturer.Value;
    private readonly Lazy<string> _manufacturer = new(() =>
        UpdateParser.CreateProperty<string>(Navigator, Manager, "@Manufacturer"));

    public string Company => _company.Value;
    private readonly Lazy<string> _company = new(() =>
        UpdateParser.CreateProperty<string>(Navigator, Manager, "@Company"));

    public string Provider => _provider.Value;
    private readonly Lazy<string> _provider = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@Provider"));

    public DateTime DriverVersionDate => _driverVersionDate.Value;
    private readonly Lazy<DateTime> _driverVersionDate = new(() =>
		UpdateParser.CreateProperty<DateTime>(Navigator, Manager, "@DriverVerDate"));

    public string DriverVersion => _driverVersion.Value;
    private readonly Lazy<string> _driverVersion = new(() =>
        UpdateParser.CreateProperty<string>(Navigator, Manager, "@DriverVerVersion"));

    public string Class => _class.Value;
    private readonly Lazy<string> _class = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@Class"));

    public string Model => _model.Value;
    private readonly Lazy<string> _model = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@Model"));

	public IReadOnlyList<FeatureScore> FeatureScores => _featureScores.Value;
    private readonly Lazy<IReadOnlyList<FeatureScore>> _featureScores = new(() => 
        UpdateParser.CreateCollection<FeatureScore>(Navigator, Manager));

    public IReadOnlyList<DistributionComputerHardwareId> DistributionComputerHardwareIds => _distributionComputerHardwareIds.Value;
    private readonly Lazy<IReadOnlyList<DistributionComputerHardwareId>> _distributionComputerHardwareIds = new(() => 
        UpdateParser.CreateCollection<DistributionComputerHardwareId>(Navigator, Manager));

    public IReadOnlyList<TargetComputerHardwareId> TargetComputerHardwareIds => _targetComputerHardwareIds.Value;
	private readonly Lazy<IReadOnlyList<TargetComputerHardwareId>> _targetComputerHardwareIds = new(() => 
        UpdateParser.CreateCollection<TargetComputerHardwareId>(Navigator, Manager));
}
