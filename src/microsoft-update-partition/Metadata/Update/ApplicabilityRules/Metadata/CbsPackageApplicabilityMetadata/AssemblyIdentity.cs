using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;
using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.ApplicabilityRules.Metadata.CbsPackageApplicabilityMetadata;

public record AssemblyIdentity(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<AssemblyIdentity>, IMetadataCollection<AssemblyIdentity>
{
	// ICreatable
	static string[] ICreatable<AssemblyIdentity>.ValidParentNodes => ["assembly"];
	static string ICreatable<AssemblyIdentity>.XPathQuery => "asm:assemblyIdentity";
	static AssemblyIdentity ICreatable<AssemblyIdentity>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	// Properties
	public string? Name => _name.Value;
	private readonly Lazy<string?> _name = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@name"));

	public string? Version => _version.Value;
	private readonly Lazy<string?> _version = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@version"));

	public string? ProcessorArchitecture => _processorArchitecture.Value;
	private readonly Lazy<string?> _processorArchitecture = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@processorArchitecture"));

	public string Language => _language.Value;
	private readonly Lazy<string> _language = new(() =>
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@language"));

	public string? BuildType => _buildType.Value;
	private readonly Lazy<string?> _buildType = new(() =>
		UpdateParser.TryCreateOptionalProperty<string>(Navigator, Manager, "@buildType", out var result) ? result : null);

	public string? PublicKeyToken => _publicKeyToken.Value;
	private readonly Lazy<string?> _publicKeyToken = new(() =>
		UpdateParser.TryCreateOptionalProperty<string>(Navigator, Manager, "@publicKeyToken", out var result) ? result : null);

}
