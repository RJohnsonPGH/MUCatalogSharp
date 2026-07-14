using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update.HandlerSpecificData;

namespace MUCatalogSharp.Tests.Metadata.Update;

public class HandlerSpecificDataTests
{
	[Theory]
	[ClassData(typeof(HandlerSpecificDataTestCbsData))]
	public void GetHandlerSpecificData_ShouldSucceed_WhenCbsData(string filePath, string packageIdentity)
	{
		// Arrange
		SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);

		// Act
		var node = navigator.SelectSingleNode("upd:Update", manager) ??
			throw new InvalidOperationException("Update node not found.");

		// Assert
		var updatehandlerSpecificData = UpdateParser.Create<UpdateHandlerSpecificData>(node, manager);
		Assert.NotNull(updatehandlerSpecificData);

		if (updatehandlerSpecificData.Handler is not CbsData cbsData)
		{
			Assert.Fail("Handler is not of type CbsData.");
			return;
		}

		Assert.Equal(packageIdentity, cbsData.PackageIdentity);
	}

	[Theory]
	[ClassData(typeof(HandlerSpecificDataTestCategoryInformationData))]
	public void GetHandlerSpecificData_ShouldSucceed_WhenCategoryInformation(string filePath, int categoryType, bool prohibitsSubcategories)
	{
		// Arrange
		SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);

		// Act
		var node = navigator.SelectSingleNode("upd:Update", manager) ??
			throw new InvalidOperationException("Update node not found.");

		// Assert
		var updatehandlerSpecificData = UpdateParser.Create<UpdateHandlerSpecificData>(node, manager);
		Assert.NotNull(updatehandlerSpecificData);

		if (updatehandlerSpecificData.Handler is not CategoryInformation categoryInformation)
		{
			Assert.Fail("Handler is not of type CategoryInformationData.");
			return;
		}

		Assert.Equal((CategoryInformation.CategoryType)categoryType, categoryInformation.Type);
		Assert.Equal(prohibitsSubcategories, categoryInformation.ProhibitsSubcategories);
	}
}

public class HandlerSpecificDataTestCbsData : TheoryData<string, string>
{
	public HandlerSpecificDataTestCbsData()
	{
		Add(@"..\..\..\..\data\sample_update_1.txt", string.Empty);
	}
}

public class HandlerSpecificDataTestCategoryInformationData : TheoryData<string, int, bool>
{
	public HandlerSpecificDataTestCategoryInformationData()
	{
		Add(@"..\..\..\..\data\sample_category_1.txt", 1, true);
	}
}
