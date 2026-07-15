namespace MUCatalogSharp.Tests.Integration;

public partial class UpstreamSourceTests
{
	//[Fact]
	//public async Task GetDriversAsync_ShouldReturnDrivers()
	//{
	//	// Arrange
	//	var driversSource = new UpstreamSource(Endpoint.Default);

	//	string manufacturer, family, productName, skuNumber;
	//	string baseBoardManufacturer, baseBoardProduct;
	//	string biosVendor, biosVersion, biosMajorRelease, biosMinorRelease;
	//	string enclosureType;

	//	//
	//	// Win32_ComputerSystem
	//	//
	//	using (var searcher = new ManagementObjectSearcher(
	//		"SELECT Manufacturer, Model, SystemFamily, SystemSKUNumber FROM Win32_ComputerSystem"))
	//	{
	//		using ManagementObject info = searcher.Get().Cast<ManagementObject>().First();

	//		manufacturer = GetValue(info, "Manufacturer");
	//		productName = GetValue(info, "Model");
	//		family = GetValue(info, "SystemFamily");
	//		skuNumber = GetValue(info, "SystemSKUNumber");
	//	}

	//	//
	//	// Win32_BaseBoard
	//	//
	//	using (var searcher = new ManagementObjectSearcher(
	//		"SELECT Manufacturer, Product FROM Win32_BaseBoard"))
	//	{
	//		using ManagementObject info = searcher.Get().Cast<ManagementObject>().First();

	//		baseBoardManufacturer = GetValue(info, "Manufacturer");
	//		baseBoardProduct = GetValue(info, "Product");
	//	}

	//	//
	//	// Win32_BIOS
	//	//
	//	using (var searcher = new ManagementObjectSearcher(
	//		"SELECT Manufacturer, SMBIOSBIOSVersion, SystemBiosMajorVersion, SystemBiosMinorVersion FROM Win32_BIOS"))
	//	{
	//		using ManagementObject info = searcher.Get().Cast<ManagementObject>().First();

	//		biosVendor = GetValue(info, "Manufacturer");
	//		biosVersion = GetValue(info, "SMBIOSBIOSVersion");

	//		// Microsoft expects these as lowercase hexadecimal bytes.
	//		biosMajorRelease = FormatBiosRelease(info["SystemBiosMajorVersion"]);
	//		biosMinorRelease = FormatBiosRelease(info["SystemBiosMinorVersion"]);
	//	}

	//	//
	//	// Win32_SystemEnclosure
	//	//
	//	using (var searcher = new ManagementObjectSearcher(
	//		"SELECT ChassisTypes FROM Win32_SystemEnclosure"))
	//	{
	//		using ManagementObject info = searcher.Get().Cast<ManagementObject>().First();

	//		// ChassisTypes is an array, get the first element
	//		enclosureType = FormatEnclosureType(info["ChassisTypes"]);
	//	}

	//	// Act - Get GUIDs from our implementation
	//	Guid[] ourGuids = UpstreamDriverFilter.CreateComputerFilter(
	//		manufacturer, family, productName, skuNumber,
	//		baseBoardManufacturer, baseBoardProduct,
	//		biosVendor, biosVersion, biosMajorRelease, biosMinorRelease, enclosureType);

	//	Guid[] tempGuid = [ourGuids[9]];
	//	string[] hardwareIds = [
	//		@"PCI\VEN_8086&DEV_7AD0"
	//	];

	//	var filter = new UpstreamDriverFilter()
	//	{
	//		ComputerFilter = tempGuid,
	//		PnpHardwareFilter = hardwareIds
	//	};

	//	// Act
	//	var drivers = new List<MicrosoftUpdatePackage>();
	//	await foreach (var driver in driversSource
	//		.GetDriversAsync(new Progress<(int, int)>(), filter, cancellationToken: TestContext.Current.CancellationToken)
	//		.ConfigureAwait(false))
	//	{
	//		drivers.Add(driver);
	//	}

	//	// Assert
	//	Assert.NotEmpty(drivers);
	//}

	//private static string GetValue(ManagementObject info, string propertyName)
	//{
	//	try
	//	{
	//		return info[propertyName]?.ToString() ?? string.Empty;
	//	}
	//	catch
	//	{
	//		return string.Empty;
	//	}
	//}

	//private static string FormatBiosRelease(object? value)
	//{
	//	if (value == null)
	//		return string.Empty;

	//	return Convert.ToByte(value)
	//		.ToString("X2")
	//		.ToLowerInvariant();
	//}

	//private static string FormatEnclosureType(object? value)
	//{
	//	if (value == null)
	//		return string.Empty;

	//	try
	//	{
	//		// ChassisTypes is an array of ushorts
	//		if (value is ushort[] types && types.Length > 0)
	//		{
	//			return types[0].ToString("x").ToLowerInvariant();
	//		}

	//		// Handle single value case
	//		if (value is ushort singleType)
	//		{
	//			return singleType.ToString("x").ToLowerInvariant();
	//		}

	//		return string.Empty;
	//	}
	//	catch
	//	{
	//		return string.Empty;
	//	}
	//}
}
