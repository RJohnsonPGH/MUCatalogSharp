using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Tests;

internal static class SampleUpdate
{
    public static IEnumerable<object[]> ValidSampleUpdatePaths = [
        [@"..\..\..\..\data\sample_update_1.txt"],
		[@"..\..\..\..\data\sample_update_2.txt"],
	];

    public static IEnumerable<object[]> ValidSampleDetectoidXmlPaths = [
		[@"..\..\..\..\data\sample_detectoid_1.txt"]
	];

	public static IEnumerable<object[]> ValidSampleCategoryXmlPaths = [
		[@"..\..\..\..\data\sample_category_1.txt"]
	];

	public static IEnumerable<object[]> InvalidSampleUpdatePaths = [
    
    ];

    public static void LoadSampleUpdateXml(string filePath, out XPathNavigator navigator, out XmlNamespaceManager manager)
    {
        var updateMetadataFileStream = File.OpenRead(filePath);
        var document = new XPathDocument(updateMetadataFileStream);
        navigator = document.CreateNavigator();

        manager = new XmlNamespaceManager(navigator.NameTable);
        manager.AddNamespace("upd", "http://schemas.microsoft.com/msus/2002/12/Update");
        manager.AddNamespace("cat", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/Category");
        manager.AddNamespace("drv", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/WindowsDriver");
        manager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        manager.AddNamespace("cmd", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/CommandLineInstallation");
        manager.AddNamespace("psf", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/WindowsPatch");
        manager.AddNamespace("cbs", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/Cbs");
        manager.AddNamespace("cbsar", "http://schemas.microsoft.com/msus/2002/12/CbsApplicabilityRules");
        manager.AddNamespace("msp", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/WindowsInstaller");
        manager.AddNamespace("wsi", "http://schemas.microsoft.com/msus/2002/12/UpdateHandlers/WindowsSetup");
		manager.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v3");
	}
}
