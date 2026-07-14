using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;

public record UpdateProperties(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<UpdateProperties>
{
	// ICreatable
	static string[] ICreatable<UpdateProperties>.ValidParentNodes => ["upd:Update"];
	static string ICreatable<UpdateProperties>.XPathQuery => "upd:Properties";
	static UpdateProperties ICreatable<UpdateProperties>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	public enum UpdateType
	{
		Detectoid,
		Category,
		Driver,
		Software
	}

	public UpdateType Type => _updateType.Value;
	private readonly Lazy<UpdateType> _updateType = new(() => 
		UpdateParser.CreateProperty<ParsableEnum<UpdateType>>(Navigator, Manager, "@UpdateType"));

	public DateTime CreationDate => _creationDate.Value;
	private readonly Lazy<DateTime> _creationDate = new(() => UpdateParser.CreateProperty<DateTime>(Navigator, Manager, "@CreationDate"));

	public string DefaultPropertiesLanguage => _defaultPropertiesLanguage.Value;
	private readonly Lazy<string> _defaultPropertiesLanguage = new(() => 
		UpdateParser.CreateProperty<string>(Navigator, Manager, "@DefaultPropertiesLanguage"));

	public long? MaxDownloadSize => _maxDownloadSize.Value;
	private readonly Lazy<long?> _maxDownloadSize = new(() => 
		UpdateParser.TryCreateOptionalProperty<long>(Navigator, Manager, "@MaxDownloadSize", out var result) ? result : null);

	public long? MinDownloadSize => _minDownloadSize.Value;
	private readonly Lazy<long?> _minDownloadSize = new(() =>
		UpdateParser.TryCreateOptionalProperty<long>(Navigator, Manager, "@MinDownloadSize", out var result) ? result : null);

	public Uri? MoreInfoUrl => _moreInfoUrl.Value;
	private readonly Lazy<Uri?> _moreInfoUrl = new(() => 
		UpdateParser.TryCreateOptionalProperty<ParsableUri>(Navigator, Manager, "@MoreInfoUrl", out var result) ? result?.Value : null);

	public Uri? SupportUrl => _supportUrl.Value;
	private readonly Lazy<Uri?> _supportUrl = new(() => 
		UpdateParser.TryCreateOptionalProperty<ParsableUri>(Navigator, Manager, "@SupportUrl", out var result) ? result?.Value : null);

	public string? KBArticleId => _kbArticleId.Value;
	private readonly Lazy<string?> _kbArticleId = new(() => 
		UpdateParser.TryCreateOptionalProperty<string>(Navigator, Manager, "@KbArticleId", out var result) ? result : null);

	public bool? OsUpgrade => _osUpgrade.Value;
	private readonly Lazy<bool?> _osUpgrade = new(() => 
		UpdateParser.TryCreateOptionalProperty<bool>(Navigator, Manager, "@OSUpgrade", out var result) ? result : null);

	public InstallationBehavior InstallationBehavior { get; init; } = new(Navigator, Manager);
	public UninstallationBehavior UninstallationBehavior { get; init; } = new(Navigator, Manager);

	public IReadOnlyList<string> Languages => _languages.Value;
	private readonly Lazy<IReadOnlyList<string>> _languages = new(() =>
		[.. UpdateParser.CreateCollection<Language>(Navigator, Manager).Select(x => x.Value)]);

}
