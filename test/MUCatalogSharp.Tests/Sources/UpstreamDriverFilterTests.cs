//using System.Diagnostics;
//using System.Management;
//using System.Text.RegularExpressions;
//using Microsoft.PackageGraph.MicrosoftUpdate.Source.Sources;

//namespace MUCatalogSharp.Tests.Sources;

//public class UpstreamDriverFilterTests
//{
//	private const string ComputerHardwareIdsPath = @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\computerhardwareids.exe";

//	[Fact]
//	public void CreateComputerFilter_MatchesComputerHardwareIdsOutput()
//	{
//		// Skip test if computerhardwareids.exe is not available
//		if (!File.Exists(ComputerHardwareIdsPath))
//		{
//			// xUnit v3 doesn't have built-in skip in theory code, so we'll just pass the test
//			// A better approach would be to add Xunit.SkippableFact package, but for now we'll skip
//			return;
//		}

//		// Arrange - Get system properties from WMI
//		string manufacturer, family, productName, skuNumber;
//		string baseBoardManufacturer, baseBoardProduct;
//		string biosVendor, biosVersion, biosMajorRelease, biosMinorRelease;
//		string enclosureType;

//		//
//		// Win32_ComputerSystem
//		//
//		using (var searcher = new ManagementObjectSearcher(
//			"SELECT Manufacturer, Model, SystemFamily, SystemSKUNumber FROM Win32_ComputerSystem"))
//		{
//			using ManagementObject info = searcher.Get().Cast<ManagementObject>().First();

//			manufacturer = GetValue(info, "Manufacturer");
//			productName = GetValue(info, "Model");
//			family = GetValue(info, "SystemFamily");
//			skuNumber = GetValue(info, "SystemSKUNumber");
//		}

//		//
//		// Win32_BaseBoard
//		//
//		using (var searcher = new ManagementObjectSearcher(
//			"SELECT Manufacturer, Product FROM Win32_BaseBoard"))
//		{
//			using ManagementObject info = searcher.Get().Cast<ManagementObject>().First();

//			baseBoardManufacturer = GetValue(info, "Manufacturer");
//			baseBoardProduct = GetValue(info, "Product");
//		}

//		//
//		// Win32_BIOS
//		//
//		using (var searcher = new ManagementObjectSearcher(
//			"SELECT Manufacturer, SMBIOSBIOSVersion, SystemBiosMajorVersion, SystemBiosMinorVersion FROM Win32_BIOS"))
//		{
//			using ManagementObject info = searcher.Get().Cast<ManagementObject>().First();

//			biosVendor = GetValue(info, "Manufacturer");
//			biosVersion = GetValue(info, "SMBIOSBIOSVersion");

//			// Microsoft expects these as lowercase hexadecimal bytes.
//			biosMajorRelease = FormatBiosRelease(info["SystemBiosMajorVersion"]);
//			biosMinorRelease = FormatBiosRelease(info["SystemBiosMinorVersion"]);
//		}

//		//
//		// Win32_SystemEnclosure
//		//
//		using (var searcher = new ManagementObjectSearcher(
//			"SELECT ChassisTypes FROM Win32_SystemEnclosure"))
//		{
//			using ManagementObject info = searcher.Get().Cast<ManagementObject>().First();

//			// ChassisTypes is an array, get the first element
//			enclosureType = FormatEnclosureType(info["ChassisTypes"]);
//		}

//		// Act - Get GUIDs from our implementation
//		Guid[] ourGuids = UpstreamDriverFilter.CreateComputerFilter(
//			manufacturer, family, productName, skuNumber,
//			baseBoardManufacturer, baseBoardProduct,
//			biosVendor, biosVersion, biosMajorRelease, biosMinorRelease, enclosureType);

//		// Act - Get GUIDs from Microsoft's computerhardwareids.exe
//		Guid[] expectedGuids = RunComputerHardwareIds();

//		// Assert
//		Assert.NotNull(ourGuids);
//		Assert.NotNull(expectedGuids);
//		Assert.Equal(15, ourGuids.Length);
//		Assert.Equal(15, expectedGuids.Length);

//		// Compare each GUID with better error messages
//		for (int i = 0; i < 15; i++)
//		{
//			Assert.True(
//				expectedGuids[i] == ourGuids[i],
//				$"GUID mismatch at index {i}:\n" +
//				$"  Expected: {expectedGuids[i]}\n" +
//				$"  Actual:   {ourGuids[i]}");
//		}
//	}

//	private static Guid[] RunComputerHardwareIds()
//	{
//		var process = new Process
//		{
//			StartInfo = new ProcessStartInfo
//			{
//				FileName = ComputerHardwareIdsPath,
//				UseShellExecute = false,
//				RedirectStandardOutput = true,
//				CreateNoWindow = true
//			}
//		};

//		process.Start();
//		string output = process.StandardOutput.ReadToEnd();
//		process.WaitForExit();

//		// Parse the Hardware IDs section
//		return ParseHardwareIds(output);
//	}

//	private static Guid[] ParseHardwareIds(string output)
//	{
//		var guids = new List<Guid>();
//		var lines = output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

//		bool inHardwareIdsSection = false;
//		var guidRegex = new Regex(@"^\{([0-9a-f\-]+)\}", RegexOptions.IgnoreCase);

//		foreach (var line in lines)
//		{
//			if (line.Trim() == "Hardware IDs")
//			{
//				inHardwareIdsSection = true;
//				continue;
//			}

//			if (line.Trim() == "------------")
//			{
//				continue;
//			}

//			if (inHardwareIdsSection && string.IsNullOrWhiteSpace(line))
//			{
//				break;
//			}

//			if (inHardwareIdsSection)
//			{
//				var match = guidRegex.Match(line.Trim());
//				if (match.Success)
//				{
//					guids.Add(Guid.Parse(match.Groups[1].Value));
//				}
//			}
//		}

//		return guids.ToArray();
//	}

//	private static string GetValue(ManagementObject info, string propertyName)
//	{
//		try
//		{
//			return info[propertyName]?.ToString() ?? string.Empty;
//		}
//		catch
//		{
//			return string.Empty;
//		}
//	}

//	private static string FormatBiosRelease(object? value)
//	{
//		if (value == null)
//			return string.Empty;

//		return Convert.ToByte(value)
//			.ToString("X2")
//			.ToLowerInvariant();
//	}

//	private static string FormatEnclosureType(object? value)
//	{
//		if (value == null)
//			return string.Empty;

//		try
//		{
//			// ChassisTypes is an array of ushorts
//			if (value is ushort[] types && types.Length > 0)
//			{
//				return types[0].ToString("x").ToLowerInvariant();
//			}

//			// Handle single value case
//			if (value is ushort singleType)
//			{
//				return singleType.ToString("x").ToLowerInvariant();
//			}

//			return string.Empty;
//		}
//		catch
//		{
//			return string.Empty;
//		}
//	}
//}

