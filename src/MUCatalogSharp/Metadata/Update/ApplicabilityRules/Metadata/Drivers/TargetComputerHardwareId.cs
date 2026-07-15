using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using System;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Update.ApplicabilityRules.Metadata.Drivers;

public sealed record TargetComputerHardwareId(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<TargetComputerHardwareId>, IMetadataCollection<TargetComputerHardwareId>
{
	static string[] ICreatable<TargetComputerHardwareId>.ValidParentNodes => ["drv:WindowsDriverMetaData"];
	static string ICreatable<TargetComputerHardwareId>.XPathQuery => "drv:TargetComputerHardwareId";
	static TargetComputerHardwareId ICreatable<TargetComputerHardwareId>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	public Guid Id => _id.Value;
	private readonly Lazy<Guid> _id = new(() =>
		UpdateParser.CreateProperty<Guid>(Navigator, Manager, "text()"));

	public static implicit operator Guid(TargetComputerHardwareId targetComputerHardwareId) => targetComputerHardwareId.Id;
}
