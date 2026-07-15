using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.ApplicabilityRules.Metadata.Drivers;

public sealed record DistributionComputerHardwareId(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<DistributionComputerHardwareId>, IMetadataCollection<DistributionComputerHardwareId>
{
	static string[] ICreatable<DistributionComputerHardwareId>.ValidParentNodes => ["drv:WindowsDriverMetaData"];
	static string ICreatable<DistributionComputerHardwareId>.XPathQuery => "drv:DistributionComputerHardwareId";
	static DistributionComputerHardwareId ICreatable<DistributionComputerHardwareId>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	public Guid Id => _id.Value;
	private readonly Lazy<Guid> _id = new(() =>
		UpdateParser.CreateProperty<Guid>(Navigator, Manager, "text()"));

	public static implicit operator Guid(DistributionComputerHardwareId distributionComputerHardwareId) => distributionComputerHardwareId.Id;
}
