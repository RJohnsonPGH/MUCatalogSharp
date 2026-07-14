using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.ApplicabilityRules;
using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;

public record UpdateApplicabilityRules(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<UpdateApplicabilityRules>
{
    static string[] ICreatable<UpdateApplicabilityRules>.ValidParentNodes => ["upd:Update"];
    static string ICreatable<UpdateApplicabilityRules>.XPathQuery => "upd:ApplicabilityRules";
    static UpdateApplicabilityRules ICreatable<UpdateApplicabilityRules>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    public IsInstalled? IsInstalled => _isInstalled.Value;
    private readonly Lazy<IsInstalled?> _isInstalled = new(() => 
        UpdateParser.CreateOptional<IsInstalled>(Navigator, Manager));

    public IsInstallable? IsInstallable => _isInstallable.Value;
    private readonly Lazy<IsInstallable?> _isInstallable = new(() => 
        UpdateParser.CreateOptional<IsInstallable>(Navigator, Manager));

    public ApplicabilityRulesMetadata? Metadata => _metadata.Value;
    private readonly Lazy<ApplicabilityRulesMetadata?> _metadata = new(() => 
        UpdateParser.CreateOptional<ApplicabilityRulesMetadata>(Navigator, Manager));

    //public WindowsVersion? WindowsVersion { get; } = UpdateParser.CreateOptional<WindowsVersion>(Navigator, Manager);
    //public IsSuperseded? IsSuperseded { get; } = UpdateParser.CreateOptional<IsSuperseded>(Navigator, Manager);
}
