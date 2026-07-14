using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.Files;

public record UpdateFile(string Digest, string DigestAlgorithm, Uri Uri, string FileName, ulong Size, DateTime Modified, string? PatchingType, IReadOnlyList<AdditionalDigest> AdditionalDigests);

public record UnresolvedUpdateFile(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<UnresolvedUpdateFile>, IMetadataCollection<UnresolvedUpdateFile>
{
    // ICreatable
    static string[] ICreatable<UnresolvedUpdateFile>.ValidParentNodes => ["upd:Files"];
    static string ICreatable<UnresolvedUpdateFile>.XPathQuery => "upd:File";
    static UnresolvedUpdateFile ICreatable<UnresolvedUpdateFile>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

    // Properties
    public string Digest => _digest.Value;
    private readonly Lazy<string> _digest = new(() => 
        UpdateParser.CreateProperty<string>(Navigator, Manager, "@Digest"));

    public string DigestAlgorithm => _digestAlgorithm.Value;
    private readonly Lazy<string> _digestAlgorithm = new(() => 
        UpdateParser.CreateProperty<string>(Navigator, Manager, "@DigestAlgorithm"));

    public string FileName => _fileName.Value;
    private readonly Lazy<string> _fileName = new(() => 
        UpdateParser.CreateProperty<string>(Navigator, Manager, "@FileName"));

    public ulong Size => _size.Value;
    private readonly Lazy<ulong> _size = new(() => 
        UpdateParser.CreateProperty<ulong>(Navigator, Manager, "@Size"));

    public DateTime Modified => _modified.Value;
    private readonly Lazy<DateTime> _modified = new(() => 
        UpdateParser.CreateProperty<DateTime>(Navigator, Manager, "@Modified"));

    public string? PatchingType => _patchingType.Value;
    private readonly Lazy<string?> _patchingType = new(() => 
        UpdateParser.TryCreateOptionalProperty<string>(Navigator, Manager, "@PatchingType", out var value) ? value : null);

    public IReadOnlyList<AdditionalDigest> AdditionalDigests => _additionalDigests.Value;
    private readonly Lazy<IReadOnlyList<AdditionalDigest>> _additionalDigests = new(() => 
        UpdateParser.CreateCollection<AdditionalDigest>(Navigator, Manager));
}
