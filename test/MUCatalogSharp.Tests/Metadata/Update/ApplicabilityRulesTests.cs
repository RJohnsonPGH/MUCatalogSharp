using MUCatalogSharp.Metadata.Parsers;
using MUCatalogSharp.Metadata.Update;

namespace MUCatalogSharp.Tests.Metadata.Update;

public class ApplicabilityRulesTests
{
    [Theory]
    [ClassData(typeof(ApplicabilityRulesTestData))]
    public void GetApplicabilityRules_ShouldSucceed_ValidXML(string filePath, bool hasIsInstalled, bool hasIsInstallable, bool hasMetadata,
        bool hasPackageApplicabilityMetadata, bool hasAssemblies)
    {
        // Arrange
        SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);

        // Act
        var node = navigator.SelectSingleNode("upd:Update", manager) ??
            throw new InvalidOperationException("Update node not found in the XML.");
        var applicabilityRules = UpdateParser.Create<UpdateApplicabilityRules>(node, manager);

        // Assert
        Assert.NotNull(applicabilityRules);
        Assert.Equal(hasIsInstalled, applicabilityRules.IsInstalled is not null);
        Assert.Equal(hasIsInstallable, applicabilityRules.IsInstallable is not null);
        Assert.Equal(hasMetadata, applicabilityRules.Metadata is not null);
        Assert.Equal(hasPackageApplicabilityMetadata, applicabilityRules.Metadata?.PackageApplicabilityMetadata is not null);
        Assert.Equal(hasAssemblies, applicabilityRules.Metadata?.PackageApplicabilityMetadata?.Assemblies is not null);
    }

    [Theory]
    [ClassData(typeof(WindowsDriverMetadataTestData))]
    public void GetWindowsDriverMetadata_ShouldSucceed_ValidXML(string filePath, string expectedHardwareId, DateTime expectedDriverVersionDate, string expectedDriverVersion,
        string expectedWhqpDriverId, string expectedClass, string expectedCompany, string expectedProvider, string expectedManufacturer, string expectedModel, int[] expectedFeatureScores, string[] expectedOperatingSystems, 
        Guid[] expectedDistributionComputerHardwareIds, Guid[] expectedTargetComputerHardwareIds)
    {
        Assert.True(expectedFeatureScores.Length == expectedOperatingSystems.Length, "Invalid test data. Feature Scores and Operating Systems arrays must have the same length.");

		// Arrange
		SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);

		// Act
		var node = navigator.SelectSingleNode("upd:Update", manager) ??
			throw new InvalidOperationException("Update node not found in the XML.");
		var applicabilityRules = UpdateParser.Create<UpdateApplicabilityRules>(node, manager);

        // Assert
        Assert.NotNull(applicabilityRules.Metadata?.WindowsDriverMetadata);
        var driverMetadata = applicabilityRules.Metadata.WindowsDriverMetadata;

		Assert.Equal(expectedHardwareId, driverMetadata.HardwareId);
		Assert.Equal(expectedDriverVersionDate, driverMetadata.DriverVersionDate);
		Assert.Equal(expectedDriverVersion, driverMetadata.DriverVersion);
        Assert.Equal(expectedWhqpDriverId, driverMetadata.WhqlDriverID);
		Assert.Equal(expectedClass, driverMetadata.Class);
        Assert.Equal(expectedCompany, driverMetadata.Company);
		Assert.Equal(expectedProvider, driverMetadata.Provider);
        Assert.Equal(expectedManufacturer, driverMetadata.Manufacturer);
        Assert.Equal(expectedModel, driverMetadata.Model);

        // Feature Scores
        Assert.Equal(expectedFeatureScores.Length, driverMetadata.FeatureScores.Count);

        for (int i = 0; i < driverMetadata.FeatureScores.Count; i++)
        {
            Assert.Equal(expectedFeatureScores[i], driverMetadata.FeatureScores[i].Score);
            Assert.Equal(expectedOperatingSystems[i], driverMetadata.FeatureScores[i].OperatingSystem);
        }

        // Distribution computer hardware IDs
        Assert.Equal(expectedDistributionComputerHardwareIds.Length, driverMetadata.DistributionComputerHardwareIds.Count);
        for (int i = 0; i < driverMetadata.DistributionComputerHardwareIds.Count; i++)
        {
            Assert.Equal(expectedDistributionComputerHardwareIds[i], driverMetadata.DistributionComputerHardwareIds[i]);
        }

		// Target computer hardware IDs
        Assert.Equal(expectedTargetComputerHardwareIds.Length, driverMetadata.TargetComputerHardwareIds.Count);
		for (int i = 0; i < driverMetadata.TargetComputerHardwareIds.Count; i++)
		{
			Assert.Equal(expectedTargetComputerHardwareIds[i], driverMetadata.TargetComputerHardwareIds[i]);
		}
	}

	[Theory]
	[ClassData(typeof(WindowsUpdateMetadataTestData))]
	public void GetWindowsUpdateMetadata_ShouldSucceed_ValidXML(string filePath, int expectedAssemblyCount, string expectedDescription, string expectedDisplayName, string expectedCompany, 
        Uri expectedSupportInformation, DateTime expectedCreationTimestamp, DateTime expectedLastUpdateTimestamp,
        string[] assemblyIdentityNames, string[] assemblyIdentityVersions, string[] assemblyIdentityProcessorArchitectures, string[] assemblyIdentityLanguages, string[] assemblyIdentityPublicKeyTokens)
	{
        Assert.True(assemblyIdentityNames.Length == assemblyIdentityVersions.Length &&
					assemblyIdentityNames.Length == assemblyIdentityProcessorArchitectures.Length &&
					assemblyIdentityNames.Length == assemblyIdentityLanguages.Length &&
					assemblyIdentityNames.Length == assemblyIdentityPublicKeyTokens.Length,
					"Invalid test data. All assembly identity arrays must have the same length.");

		// Arrange
		SampleUpdate.LoadSampleUpdateXml(filePath, out var navigator, out var manager);

		// Act
		var node = navigator.SelectSingleNode("upd:Update", manager) ??
			throw new InvalidOperationException("Update node not found in the XML.");
		var applicabilityRules = UpdateParser.Create<UpdateApplicabilityRules>(node, manager);

		// Assert
		Assert.NotNull(applicabilityRules.Metadata?.PackageApplicabilityMetadata);
		var metadata = applicabilityRules.Metadata.PackageApplicabilityMetadata;

		Assert.Equal(expectedAssemblyCount, metadata.Assemblies.Count);
        var assembly = metadata.Assemblies[0];
        Assert.Equal(expectedDescription, assembly.Description);
		Assert.Equal(expectedDisplayName, assembly.DisplayName);
		Assert.Equal(expectedCompany, assembly.Company);
        Assert.Equal(expectedSupportInformation, assembly.SupportInformation);
		Assert.Equal(expectedCreationTimestamp, assembly.CreationTimestamp);
		Assert.Equal(expectedLastUpdateTimestamp, assembly.LastUpdateTimestamp);

        for (int i = 0; i < assemblyIdentityNames.Length; i++)
		{
			var assemblyIdentity = metadata.Assemblies[i].AssemblyIdentity;
			Assert.NotNull(assemblyIdentity);
			Assert.Equal(assemblyIdentityNames[i], assemblyIdentity.Name);
			Assert.Equal(assemblyIdentityVersions[i], assemblyIdentity.Version);
			Assert.Equal(assemblyIdentityProcessorArchitectures[i], assemblyIdentity.ProcessorArchitecture);
			Assert.Equal(assemblyIdentityLanguages[i], assemblyIdentity.Language);
			Assert.Equal(assemblyIdentityPublicKeyTokens[i], assemblyIdentity.PublicKeyToken);
		}
	}
}

public class WindowsUpdateMetadataTestData : TheoryData<string, int, string, string, string, Uri, DateTime, DateTime, string[], string[], string[], string[], string[]>
{
	public WindowsUpdateMetadataTestData()
	{
		Add(@"..\..\..\..\data\sample_update_1.txt", 
            5145, 
            "Fix for KB5006674", 
            "default", 
            "Microsoft Corporation", 
            new Uri("https://support.microsoft.com/help/5006674"), 
            DateTime.Parse("2021-10-05T22:06:25Z"), 
            DateTime.Parse("2021-10-05T22:06:25Z"),
            ["Package_for_RollupFix"],
            ["22000.258.1.5"],
            ["arm64"],
            ["neutral"],
            ["31bf3856ad364e35"]
		);
	}
}

public class ApplicabilityRulesTestData : TheoryData<string, bool, bool, bool, bool, bool>
{
	public ApplicabilityRulesTestData()
	{
		Add(@"..\..\..\..\data\sample_update_1.txt", true, true, true, true, true);
		Add(@"..\..\..\..\data\sample_detectoid_1.txt", true, false, false, false, false);
	}
}

public class WindowsDriverMetadataTestData : TheoryData<string, string, DateTime, string, string, string, string, string, string, string, int[], string[], Guid[], Guid[]>
{
	public WindowsDriverMetadataTestData()
    {
	    Add(@"..\..\..\..\data\sample_driver_1.txt", 
            @"USB\VID_8087&PID_0A2B&REV_0010",
            DateTime.Parse("2022-05-30"),
			"22.150.0.6",
            "0",
			"Other Hardware",
			"Intel Corporation",
			"Intel Corporation",
            "Intel Corporation",
			"Intel(R) Wireless Bluetooth(R)",
            [255],
            ["amd64.10.0"],
            [
                new("3962b55f-f768-58cb-9ca2-b5d61a1fdbae"),
                new("3e4626d6-ac14-5658-8306-27a8761ca92c"),
                new("32650cd2-2a71-53d5-b344-699e64484919"),
                new("524f555f-ce80-575c-803d-8d74aeacb857"),
                new("f860d14a-a4a9-5d3a-b60f-9796cc666200"),
            ],
            []
		);
    }
}
