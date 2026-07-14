using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers.Interfaces;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.HandlerSpecificData;
using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;

public record UpdateHandlerSpecificData(XPathNavigator Navigator, XmlNamespaceManager Manager) : ICreatable<UpdateHandlerSpecificData>
{
    public static string[] ValidParentNodes => ["upd:Update"];
    public static string XPathQuery => "upd:HandlerSpecificData";

    static UpdateHandlerSpecificData ICreatable<UpdateHandlerSpecificData>.Create(XPathNavigator navigator, XmlNamespaceManager manager) => new(navigator, manager);

	public IHandler Handler => _handler.Value;
	private readonly Lazy<IHandler> _handler = new(() =>
	{
		var handlerType = UpdateParser.CreateProperty<string>(Navigator, Manager, "@xsi:type");
		return handlerType.ToLowerInvariant() switch
		{
			"cat:category" => UpdateParser.Create<CategoryInformation>(Navigator, Manager),
			"cbs:cbs" => UpdateParser.Create<CbsData>(Navigator, Manager),
			// "cmd:commandlineinstallation" => UpdateParser.Create<CommandLineInstallation>(Navigator, Manager),
			// "msp:windowsinstallerapp" => UpdateParser.Create<WindowsInstallerApp>(Navigator, Manager),
			// "msp:windowsinstaller" => UpdateParser.Create<WindowsInstaller>(Navigator, Manager),
			// "osinstallermetadata" => UpdateParser.Create<OSInstallerMetadata>(Navigator, Manager),
			// "psf:windowspatch" => UpdateParser.Create<WindowsPatch>(Navigator, Manager),
			// "wsi:windowsetup" => UpdateParser.Create<WindowsSetup>(Navigator, Manager),
			_ => throw new InvalidOperationException($"Unknown handler type: {handlerType}"),
		};
	});
}

public interface IHandler { }
