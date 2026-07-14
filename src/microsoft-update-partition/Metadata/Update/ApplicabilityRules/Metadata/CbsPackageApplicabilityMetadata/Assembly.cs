using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;
using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.ApplicabilityRules.Metadata.CbsPackageApplicabilityMetadata;

public record Assembly(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<Assembly>, IMetadataCollection<Assembly>
{
    // ICreatable
    static string[] ICreatable<Assembly>.ValidParentNodes => ["cbsar:CbsPackageApplicabilityMetadata"];
    static string ICreatable<Assembly>.XPathQuery => "asm:assembly";
    static Assembly ICreatable<Assembly>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    // Properties
    public string? Description => _description.Value;
    private readonly Lazy<string?> _description = new(() => 
        UpdateParser.TryCreateOptionalProperty<string>(Navigator, Manager, "@description", out var result) ? result : null);

    public string? DisplayName => _displayName.Value;
    private readonly Lazy<string?> _displayName = new(() => 
        UpdateParser.TryCreateOptionalProperty<string>(Navigator, Manager, "@displayName", out var result) ? result : null);

    public string? Company => _company.Value;
    private readonly Lazy<string?> _company = new(() => 
        UpdateParser.TryCreateOptionalProperty<string>(Navigator, Manager, "@company", out var result) ? result : null);

    public Uri? SupportInformation => _supportInformation.Value;
    private readonly Lazy<Uri?> _supportInformation = new(() => 
        UpdateParser.TryCreateOptionalProperty<ParsableUri>(Navigator, Manager, "@supportInformation", out var result) ? result.Value : null);

    public DateTime? CreationTimestamp => _creationTimestamp.Value;
    private readonly Lazy<DateTime?> _creationTimestamp = new(() => 
        UpdateParser.TryCreateOptionalProperty<DateTime>(Navigator, Manager, "@creationTimeStamp", out var result) ? result : null);

    public DateTime? LastUpdateTimestamp => _lastUpdateTimestamp.Value;
    private readonly Lazy<DateTime?> _lastUpdateTimestamp = new(() => 
        UpdateParser.TryCreateOptionalProperty<DateTime>(Navigator, Manager, "@lastUpdateTimeStamp", out var result) ? result : null);

    public AssemblyIdentity? AssemblyIdentity => _assemblyIdentity.Value;
    private readonly Lazy<AssemblyIdentity?> _assemblyIdentity = new(() =>
		UpdateParser.Create<AssemblyIdentity>(Navigator, Manager));
}
