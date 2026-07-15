using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Update;

namespace MUCatalogSharp.Tests.Metadata.Update;

public class UpdateIdentityTests
{
    [Theory]
    [ClassData(typeof(UpdateIdentityTestData))]
    public void GetUpdateIdentity_ShouldSucceed_ValidXML(string filePath, Guid expectedId, int expectedRevision)
    {
        // Arrange
        SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);
        var node = navigator.SelectSingleNode("upd:Update", manager)
            ?? throw new InvalidOperationException("Failed to select the update node.");

        // Act
        var updateIdentity = UpdateParser.Create<UpdateIdentity>(node, manager);

        // Assert
        Assert.Equal(expectedId, updateIdentity.Id);
        Assert.Equal(expectedRevision, updateIdentity.Revision);
    }
}

public class UpdateIdentityTestData : TheoryData<string, Guid, int>
{
	public UpdateIdentityTestData()
	{
		Add(@"..\..\..\..\data\sample_update_1.txt", new("de574445-f9c7-4335-9f41-a69710f5eec8"), 201);
		Add(@"..\..\..\..\data\sample_update_2.txt", new("7b6d13c6-6401-4a10-a61f-1dab8d93356e"), 200);
	}
}
