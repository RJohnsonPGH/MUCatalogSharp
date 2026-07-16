using MUCatalogSharp.Metadata.Update;

namespace MUCatalogSharp.Tests.Metadata.Update;

public class UpdateRelationshipsTests
{
    [Theory]
    [ClassData(typeof(RelationshipTestData))]
    public void GetUpdatePrerequisites_ShouldSucceed_ValidXML(string filePath, Guid[] supersededUpdateIds, Guid[] prerequisiteUpdateIds, Guid[] atLeastOnePrerequisiteIds, Guid[] bundledUpdateIds, int[] bundledUpdateRevisions)
    {
        // Arrange
        SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);

        // Act
        var node = navigator.SelectSingleNode("upd:Update/upd:Relationships", manager) ??
            throw new InvalidOperationException("Failed to find the 'upd:Relationships' node.");
        var updateRelationships = new UpdateRelationships(node, manager);

        // Assert
        Assert.NotNull(updateRelationships);

        // Check superseded updates
        if (supersededUpdateIds.Length > 0)
        {
            Assert.NotNull(updateRelationships.SupersededUpdates);
            Assert.Equal(supersededUpdateIds.Length, updateRelationships.SupersededUpdates.Count);
			for (var i = 0; i < updateRelationships.SupersededUpdates.Count; i++)
            {
                var currentUpdate = updateRelationships.SupersededUpdates[i];
                Assert.Equal(supersededUpdateIds[i], currentUpdate.Id);
            }
        }

		// Check prerequisites
        if (prerequisiteUpdateIds.Length > 0)
        {
            Assert.NotNull(updateRelationships.Prerequisites);
            Assert.Equal(prerequisiteUpdateIds.Length, updateRelationships.Prerequisites.Count);
			for (var i = 0; i < updateRelationships.Prerequisites.Count; i++)
			{
				var currentUpdate = updateRelationships.Prerequisites[i];
				Assert.Equal(prerequisiteUpdateIds[i], currentUpdate.Id);
			}
		}

		// Check at least one prerequisites
        if (atLeastOnePrerequisiteIds.Length > 0)
        {
            Assert.NotNull(updateRelationships.Prerequisites.AtLeastOnePrerequisites);
            Assert.Equal(atLeastOnePrerequisiteIds.Length, updateRelationships.Prerequisites.AtLeastOnePrerequisites.Count);
			for (var i = 0; i < updateRelationships.Prerequisites.AtLeastOnePrerequisites.Count; i++)
            {
                var currentAtLeastOne = updateRelationships.Prerequisites.AtLeastOnePrerequisites[i];
				Assert.Equal(atLeastOnePrerequisiteIds[i], currentAtLeastOne.UpdateIdentities[0].Id);
			}
        }

        // Bundled updates
        if (bundledUpdateIds.Length > 0)
        {
            Assert.NotNull(updateRelationships.BundledUpdates);
            Assert.Equal(bundledUpdateIds.Length, updateRelationships.BundledUpdates.Count);
            for (var i = 0; i < updateRelationships.BundledUpdates.Count; i++)
            {
                var currentBundledUpdate = updateRelationships.BundledUpdates[i];
                Assert.Equal(bundledUpdateIds[i], currentBundledUpdate.Id);
                Assert.Equal(bundledUpdateRevisions[i], currentBundledUpdate.Revision);
            }
        }
    }
}

public class RelationshipTestData : TheoryData<string, Guid[], Guid[], Guid[], Guid[], int[]>
{
	public RelationshipTestData()
    {
		Add(@"..\..\..\..\data\sample_update_1.txt",
			[],
			[
				new("c57b3cff-f20d-4841-9970-cfc10d156057"),
				new("4103af66-247a-4782-b970-8899394c27c3")
			],
			[
				new("72e7624a-5b00-45d2-b92f-e561c0a6a160"),
            ],
			[],
			[]
		);
		Add(@"..\..\..\..\data\sample_update_2.txt",
            [
                new("8cf5c03f-1b45-4d8f-a6d6-9fc9a927f92a"),
                new("e2ad983b-4ada-44ed-a1fd-748fbf935fb0"),
                new("7cb352fb-1eb6-4d5b-ba0b-5c39d1abfbdd"),
                new("3806f89b-e60e-4144-a1f7-9c8d67d1d84d"),
                new("54daf15a-aa18-4880-b98a-535c513ea2e4"),
                new("f7a60bf7-e59b-4d2c-92ba-5fe0af0e8ff7"),
                new("b975b9b2-6309-438e-9e7c-a7f17bb73d9b"),
                new("d657015b-8ec8-49ea-9802-6ba83ab978e2"),
                new("4ecc60a2-2dd3-4673-8c07-b9922334f6d8"),
                new("e5107e93-4dfd-4c25-af16-312e973d5dc7"),
                new("962fde09-7703-4000-aade-bf210f5b53dc"),
                new("d41e2f0f-8d87-4ce6-9e4c-248d568394d5"),
                new("43d90892-e51c-40e8-a9df-6f81b8e87f64"),
                new("7fed3ab5-0935-4202-872f-2ec135e935e9")
			],
			[
				new("c57b3cff-f20d-4841-9970-cfc10d156057"),
				new("59653007-e2e9-4f71-8525-2ff588527978")
            ],
            [
                new("0fa1201d-4330-4fa8-8ae9-b877473b6441"),
                new("72e7624a-5b00-45d2-b92f-e561c0a6a160")
            ],
            [
                new("ad1a3f81-2318-4a4d-8a86-3915edc389ae")],
            [
                200
            ]
        );
    }
}