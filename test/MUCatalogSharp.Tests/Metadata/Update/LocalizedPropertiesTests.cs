using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;

namespace MUCatalogSharp.Tests.Metadata.Update;

public class LocalizedPropertiesTests
{
    [Theory]
    [ClassData(typeof(LocalizedPropertiesTestData))]
	public void GetLocalizedPropertyCollection_ShouldSucceed_ValidXML(string filePath, int count, string language, string title, string? description)
    {
        // Arrange
        SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);

        // Act
        var node = navigator.SelectSingleNode("upd:Update", manager)
            ?? throw new InvalidOperationException("Update node not found.");
        var localizezdPropertiesCollection = UpdateParser.Create<LocalizedPropertiesCollection>(node, manager);

        // Assert
        Assert.NotNull(localizezdPropertiesCollection);
        Assert.Equal(count, localizezdPropertiesCollection.Count);
		Assert.Equal(language, localizezdPropertiesCollection[0].Language);
        Assert.Equal(title, localizezdPropertiesCollection[0].Title);
        Assert.Equal(description, localizezdPropertiesCollection[0].Description);
    }
}

public class LocalizedPropertiesTestData : TheoryData<string, int, string, string, string?>
{
    public LocalizedPropertiesTestData()
    {
        Add(@"..\..\..\..\data\sample_update_1.txt",
            1,
            "en",
			"Windows10.0-KB5006674-arm64.cab",
            null);
		Add(@"..\..\..\..\data\sample_update_2.txt",
            29,
            "en",
			"2022-05 Cumulative Update for Windows 11 for x64-based Systems (KB5013943)",
			"Install this update to resolve issues in Windows. For a complete listing of the issues that are included in this update, see the associated Microsoft Knowledge Base article for more information. After you install this item, you may have to restart your computer.");
    }
}
