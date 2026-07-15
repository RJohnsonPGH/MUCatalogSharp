using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Update;

namespace MUCatalogSharp.Tests.Metadata.Update;

public class UpdatePropertiesTests
{
	[Theory]
	[ClassData(typeof(UpdatePropertyTestData))]
	public void GetUpdateProperty_ShouldSucceed_ValidXML(string filePath, int updateType, string defaultLanguage, int? maxDownloadSize, int? minDownloadSize, DateTime creationDate)
	{
		// Arrange
		SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);

		// Act
		var node = navigator.SelectSingleNode("upd:Update", manager) ??
			throw new InvalidOperationException("Failed to select the update node.");
		var updateProperties = UpdateParser.Create<UpdateProperties>(node, manager);

		// Assert
		Assert.Equal((UpdateProperties.UpdateType)updateType, updateProperties.Type);
		Assert.Equal(defaultLanguage, updateProperties.DefaultPropertiesLanguage);
		Assert.Equal(maxDownloadSize, updateProperties.MaxDownloadSize);
		Assert.Equal(minDownloadSize, updateProperties.MinDownloadSize);
		Assert.Equal(creationDate, updateProperties.CreationDate);
	}

	[Theory]
	[ClassData(typeof(UpdatePropertiesLanguageTestData))]
	public void GetUpdateProperty_ShouldSucceed_ValidXML_Language(string filePath, string[] expectedLanguages)
	{
		// Arrange
		SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);

		// Act
		var node = navigator.SelectSingleNode("upd:Update", manager) ??
			throw new InvalidOperationException("Failed to select the update node.");
		var updateProperties = UpdateParser.Create<UpdateProperties>(node, manager);

		// Assert
		Assert.Equal(expectedLanguages.Length, updateProperties.Languages.Count);

		for (var i = 0; i < expectedLanguages.Length; i++)
		{
			Assert.Equal(expectedLanguages[i], updateProperties.Languages[i]);
		}
	}
}

public class UpdatePropertyTestData : TheoryData<string, int, string, int?, int?, DateTime>
{
	public UpdatePropertyTestData()
	{
		Add(@"..\..\..\..\data\sample_update_1.txt", 3, "en", 228728655, 0, DateTime.Parse("2021-10-11T18:19:44.768Z"));
		Add(@"..\..\..\..\data\sample_update_2.txt", 3, "en", null, null, DateTime.Parse("2022-05-10T17:00:06.000Z"));
	}
}

public class UpdatePropertiesLanguageTestData : TheoryData<string, string[]>
{
    public UpdatePropertiesLanguageTestData()
	{
		Add(@"..\..\..\..\data\sample_driver_1.txt",
			[
				"ar",
				"pt-br",
				"bg",
				"zh-hk",
				"zh-cn",
				"zh-tw",
				"hr",
				"cs",
				"da",
				"nl",
				"en",
				"et",
				"fi",
				"fr",
				"de",
				"el",
				"he",
				"hu",
				"it",
				"ja",
				"ko",
				"lv",
				"lt",
				"no",
				"pl",
				"pt",
				"ro",
				"ru",
				"sr",
				"sk",
				"sl",
				"es",
				"sv",
				"th",
				"tr",
				"uk",
			]
		);
	}
}