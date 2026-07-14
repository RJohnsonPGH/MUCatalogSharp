using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Update;

namespace MUCatalogSharp.Tests.Metadata.Update;

public class UpdateFilesTests
{
    [Theory]
    [ClassData(typeof(UpdateFilesTestsData))]
    public void GetUpdateFiles_ShouldSucceed_ValidXML(string filePath, string[] digest, string[] digestAlgorithm, string[] fileName, ulong[] size, DateTime[] modified, string[] patchingType)
    {
        Assert.True(digest.Length == digestAlgorithm.Length && 
            digest.Length == fileName.Length && 
            digest.Length == size.Length && 
            digest.Length == modified.Length && 
            digest.Length == patchingType.Length, "Test data array lengths do not match.");

        // Arrange
        SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);

		// Act
		var node = navigator.SelectSingleNode("upd:Update", manager)
			?? throw new InvalidOperationException("Failed to select the update node.");
		var files = UpdateParser.Create<UpdateFilesCollection>(node, manager);

		// Assert
		Assert.NotNull(files);
        Assert.Equal(digest.Length, files.Count);

        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            Assert.Equal(digest[i], file.Digest);
			Assert.Equal(digestAlgorithm[i], file.DigestAlgorithm);
			Assert.Equal(fileName[i], file.FileName);
			Assert.Equal(size[i], file.Size);
			Assert.Equal(modified[i], file.Modified);
			Assert.Equal(patchingType[i], file.PatchingType);
		}
    }
}

public class UpdateFilesTestsData : TheoryData<string, string[], string[], string[], ulong[], DateTime[], string[]>
{
	public UpdateFilesTestsData()
	{
		Add(@"..\..\..\..\data\sample_update_1.txt",
			["5p0cHfufwgyrzvBZKbR2E7z6qms="],
            ["SHA1"],
            ["Windows10.0-KB5006674-arm64.cab"],
			[228728655],
            [DateTime.Parse("2021-10-07T17:32:51Z")],
			["SelfContained"]
		);
	}
}