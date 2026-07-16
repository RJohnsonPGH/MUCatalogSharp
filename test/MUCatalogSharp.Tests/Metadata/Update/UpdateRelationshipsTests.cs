using MUCatalogSharp.Metadata.Update;

namespace MUCatalogSharp.Tests.Metadata.Update;

public class UpdateRelationshipsTests
{
    [Theory]
    [ClassData(typeof(RelationshipTestData))]
    public void GetUpdatePrerequisites_ShouldSucceed_ValidXML(string filePath, Guid[] supersededUpdateIds, 
		Guid[] prerequisiteUpdateIds, Guid[] atLeastOnePrerequisiteIds, 
		Guid[] bundledUpdateIds, int[] bundledUpdateRevisions, 
		Guid[] atLeastOneBundledIds, int[] atLeastOneBundledRevisions)
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

		// Bundled at least one
		if (atLeastOneBundledIds.Length > 0)
		{
			Assert.NotNull(updateRelationships.BundledUpdates?.AtLeastOneBundled);
			Assert.Equal(atLeastOneBundledIds.Length, updateRelationships.BundledUpdates.AtLeastOneBundled.Count);
			for (var i = 0; i < updateRelationships.BundledUpdates.AtLeastOneBundled.Count; i++)
			{
				var currentAtLeastOneBundled = updateRelationships.BundledUpdates.AtLeastOneBundled[i];
				Assert.Equal(atLeastOneBundledIds[i], currentAtLeastOneBundled.UpdateIdentities[0].Id);
				Assert.Equal(atLeastOneBundledRevisions[i], currentAtLeastOneBundled.UpdateIdentities[0].Revision);
			}
		}

    }
}

public class RelationshipTestData : TheoryData<string, Guid[], Guid[], Guid[], Guid[], int[], Guid[], int[]>
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
			[],
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
            ],
			[],
			[]
        );
        Add(@"..\..\..\..\data\sample_update_3.txt",
			[
				new("90a40dd7-a198-4c1a-8ed9-df0a229d8565"),
				new("a3000c2f-dc56-4eb1-9ddb-3d36fe3baf41"),
				new("d40c9962-f30e-42e6-be83-ad8b4dc04031"),
				new("ef017f17-a0e4-43a2-b6ee-b1d501fbf282"),
				new("00b3356d-e516-4890-b430-c786efe4a1b6"),
				new("616dbd3b-2c81-4f4b-9f0b-944fe40d9a13"),
				new("8cea1349-3ff8-4482-af64-9db80cb71ffb"),
				new("02276e78-d310-4617-b4e0-39bb63fc7991"),
				new("d24b928d-6733-4faf-a7cd-0b396664efda"),
				new("e59933ce-6913-4beb-9563-15665679ec92"),
				new("4e985229-0105-412d-b51f-375761b88d42"),
				new("69b15496-b618-4036-99c2-142ebbf43191"),
				new("31a79a9f-e8c1-42c1-b516-0d4de9011598"),
				new("badfc4e8-0b2e-4bc0-9ece-ae076db7e80c"),
				new("3d6862a8-c4e0-486a-bb4b-f8a62b2deb13"),
				new("9b4997fe-eb30-46b5-b15d-24e1b084ad1f"),
				new("1ec279ba-5910-4990-a198-788ad65e8fe9"),
				new("6bb2269f-e455-4507-81ad-318643a97c5f"),
				new("ad322d0e-cdf1-4cf5-8d30-1d1c24bf9430"),
				new("12dd009e-8e58-469a-ba76-743e7ba68c61"),
				new("e14b8468-d7c5-4858-9288-db31089a1ca7"),
				new("25d351be-b4b1-45e0-b12f-3c113d25aaf5"),
				new("3e99b948-dfab-473f-818f-664030c8103f"),
				new("c8ba9be3-634b-4d85-8320-9a6850ef4439"),
				new("a10de9b5-74d8-4c60-86ba-f64d3185c6e1"),
				new("2609130a-fa7e-48b3-b60f-7d66b92ddefe"),
				new("d6139262-889e-40e7-92e9-dc53dafd6dfb"),
				new("a9754743-c096-42a7-ae27-abe75938faa5"),
				new("1803e452-c199-4655-aaec-3d83cb71830b"),
				new("aed7f6e7-01e6-4265-9b39-2a665bfbfe93"),
				new("c8187267-a6f9-4b76-b6a3-a726eada437c"),
				new("92061378-be93-4659-a72a-037225e6bb0f"),
				new("7bb9caea-f866-4922-ac8f-1c2d9051f361"),
				new("bb036652-09c0-490b-bee0-143c49d147a5"),
				new("901e436e-3c6a-4542-96f2-e957e7c34ad7"),
				new("363be3e9-2a1d-4c47-af2b-ae997c7863b8"),
				new("73ed1a07-9926-4122-9ae0-6629e887596d"),
				new("acea5abe-df36-4bd9-93f8-ecbb1fed182c"),
				new("9164090f-5427-4cdb-b524-f10495a0249d"),
				new("fbbaa0b2-083b-498d-9736-6d2f914f6b34"),
				new("16261f61-fa2d-46a4-bc56-7452f02715f5"),
				new("456629a7-57c2-41c7-a1f3-4e24b3851ebf"),
				new("ee3d478c-76c1-47ed-9749-c2e814f16001"),
				new("bd55cfbb-79df-4dfc-a8f7-2a25c74bf703"),
				new("d11883f1-a0c9-4afc-b1bb-2f3d5bf28a7f"),
				new("c969c652-75b3-4ea9-a314-c69ebb26483d"),
				new("6a82df81-51b3-4ef8-9a99-fc3e85538aca"),
				new("06b76d9b-402d-4b14-bb0e-ac7536c0b6ba"),
				new("7fbb596d-bbc8-49e3-a0ed-ac185cf55ddb"),
				new("c51d5da7-a3cf-4ec0-9568-341786e5f03d"),
				new("dcb2f021-bbd9-4019-8bc3-c191984da63f"),
				new("c1c2a099-be0e-411d-a390-1a35d61348e3"),
				new("10b42b94-4b75-4bd4-aae1-326cd74ddcd0"),
				new("e1ad697b-da5b-464f-93cf-567226606082"),
				new("ba69c60d-35cb-417c-ae5a-cdc8af5ad2e0"),
				new("9bf409eb-c9b9-4c10-997a-e9f6414cd94a"),
				new("806457f6-4a6a-4ed3-af2a-35595960acfa"),
				new("970507bd-0d5e-4270-9a6f-0aa226afa1b2"),
				new("28bd461d-1140-4748-aa2e-0a03a6c4fc32"),
				new("a1e7fb49-a9ba-4cde-bc83-a906725af529"),
				new("012823a1-c6d1-4769-b7ea-c49fa226b316"),
				new("d7d5c8fb-5c5e-4089-96ea-1d3ec2d76795"),
				new("9dcced7b-ed4d-49c3-b26e-65363adc4670"),
				new("1248d549-d317-493f-a1db-f79fc8c0db48"),
				new("ae236bdb-8a3f-4a6a-b0dd-5c0b085bcd66"),
				new("1423492c-fbbb-4652-a5ac-01cfe0e757b6"),
				new("e327d2fc-31d7-459f-8950-cca4c97566c4"),
				new("7e6cc676-cc0c-4373-b32c-cec2f5b1f285"),
				new("c19769c6-d589-4dac-a798-977e5e97fd57"),
				new("0c012f79-aa29-4982-aa6d-f1c1058d25ba"),
				new("4b9492eb-7aff-4117-b896-cecd59e61bc8"),
				new("2b4502e0-41cd-4ed8-8714-369c1042cb18"),
				new("f052a7b7-940d-41d1-abf2-fd9a666baea3"),
				new("88bee16b-4ff8-486e-8ff2-e7a148080ad3"),
				new("90316cb0-9dfb-4e05-95df-3a29334d699f"),
			],
			[],
			[
				new("72e7624a-5b00-45d2-b92f-e561c0a6a160"),
				new("0fa1201d-4330-4fa8-8ae9-b877473b6441"),
				new("e5555fe7-924e-4ba0-9039-62ffcd16cf1a"),
				new("03aa8476-15b7-4288-bf9b-286b31f99bd6")
			],
			[],
			[],
			[
				new("6609771a-1e19-4e99-bc29-38ef76331070")
			],
			[
				100
			]
		);
    }
}