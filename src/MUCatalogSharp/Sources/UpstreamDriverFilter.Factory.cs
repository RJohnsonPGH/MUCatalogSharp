using System.Security.Cryptography;
using System.Text;

namespace MUCatalogSharp.Sources;

public sealed partial class UpstreamDriverFilter
{
	// https://learn.microsoft.com/en-us/windows-hardware/drivers/install/specifying-hardware-ids-for-a-computer
	// https://stackoverflow.com/questions/71513024/generate-the-same-computer-hardware-id-chid-like-computerhardwareids-exe-from

	/// <summary>
	/// Creates a computer filter based on the specified parameters. The filter is used to retrieve driver IDs that match the specified computer hardware configuration.
	/// </summary>
	/// <param name="manufacturer">Can be taken from the 'Manufacturer' field of Win32_ComputerSystem</param>
	/// <param name="family">Can be taken from the 'SystemFamily' field of Win32_ComputerSystem</param>
	/// <param name="productName">Can be taken from the 'Model' field of Win32_ComputerSystem</param>
	/// <param name="skuNumber">Can be taken from the 'SystemSKUNumber' field of Win32_ComputerSystem</param>
	/// <param name="baseBoardManufacturer">Can be taken from the 'Manufacturer' field of Win32_BaseBoard</param>
	/// <param name="baseBoardProduct">Can be taken from the 'Product' field of Win32_BaseBoard</param>
	/// <param name="biosVendor">Can be taken from the 'Manufacturer' field of Win32_BIOS</param>
	/// <param name="biosVersion">Can be taken from the 'SMBIOSBIOSVersion' field of Win32_BIOS</param>
	/// <param name="biosMajorRelease">Can be taken from the 'SystemBiosMajorVersion' field of Win32_BIOS</param>
	/// <param name="biosMinorRelease">Can be taken from the 'SystemBiosMinorVersion' field of Win32_BIOS</param>
	/// <param name="enclosureType">Can be taken from the 'ChassisTypes' field of Win32_SystemEnclosure (first element, formatted as lowercase hex)</param>
	/// <returns></returns>
	public static Guid[] CreateComputerFilter(string manufacturer, string family, string productName, string skuNumber, string baseBoardManufacturer, string baseBoardProduct,
		string biosVendor, string biosVersion, string biosMajorRelease, string biosMinorRelease, string enclosureType)
	{
        string[] hardwareIds = [
            // 0
			$"{manufacturer}&{family}&{productName}&{skuNumber}&{biosVendor}&{biosVersion}&{biosMajorRelease}&{biosMinorRelease}",
            
            // 1
			$"{manufacturer}&{family}&{productName}&{biosVendor}&{biosVersion}&{biosMajorRelease}&{biosMinorRelease}",

			// 2
			$"{manufacturer}&{productName}&{biosVendor}&{biosVersion}&{biosMajorRelease}&{biosMinorRelease}",

			// 3
			$"{manufacturer}&{family}&{productName}&{skuNumber}&{baseBoardManufacturer}&{baseBoardProduct}",

			// 4
			$"{manufacturer}&{family}&{productName}&{skuNumber}",

			// 5
			$"{manufacturer}&{family}&{productName}",

			// 6
			$"{manufacturer}&{skuNumber}&{baseBoardManufacturer}&{baseBoardProduct}",

			// 7
			$"{manufacturer}&{skuNumber}",

			// 8
			$"{manufacturer}&{productName}&{baseBoardManufacturer}&{baseBoardProduct}",

			// 9
			$"{manufacturer}&{productName}",

			// 10
			$"{manufacturer}&{family}&{baseBoardManufacturer}&{baseBoardProduct}",

			// 11
			$"{manufacturer}&{family}",

			// 12
			$"{manufacturer}&{enclosureType}",

			// 13
			$"{manufacturer}&{baseBoardManufacturer}&{baseBoardProduct}",

			// 14
            manufacturer,
		];

        return Array.ConvertAll(hardwareIds, GenerateUuidV5);
	}

	private static Guid GenerateUuidV5(string name)
	{
        var namespaceId = new Guid("70ffd812-4c7f-4c7d-0000-000000000000");
		byte[] namespaceBytes = namespaceId.ToByteArray();
		SwapByteOrder(namespaceBytes);

		byte[] nameBytes = Encoding.Unicode.GetBytes(name);

		byte[] hash;
		using (SHA1 sha1 = SHA1.Create())
		{
			sha1.TransformBlock(
				namespaceBytes,
				0,
				namespaceBytes.Length,
				null,
				0);

			sha1.TransformFinalBlock(
				nameBytes,
				0,
				nameBytes.Length);

			hash = sha1.Hash!;
		}

		byte[] newGuid = new byte[16];
		Array.Copy(hash, newGuid, 16);

		// UUID version 5
		newGuid[6] =
			(byte)((newGuid[6] & 0x0F) | 0x50);

		// RFC 4122 variant
		newGuid[8] =
			(byte)((newGuid[8] & 0x3F) | 0x80);


		SwapByteOrder(newGuid);

		return new Guid(newGuid);
	}

	private static void SwapByteOrder(byte[] guid)
	{
		(guid[0], guid[3]) = (guid[3], guid[0]);
		(guid[1], guid[2]) = (guid[2], guid[1]);
		(guid[4], guid[5]) = (guid[5], guid[4]);
		(guid[6], guid[7]) = (guid[7], guid[6]);
	}

	public enum ComputerHardwareIdMicrosoftType
    {
        /// <summary>
        /// HardwareID-0
        /// Manufacturer + Family + Product Name + SKU Number + BIOS Vendor + BIOS Version + BIOS Major Release + BIOS Minor Release
        /// </summary>
        HardwareID_0 = 0,

         /// <summary>
         /// HardwareID-1
         /// Manufacturer + Family + Product Name + BIOS Vendor + BIOS Version + BIOS Major Release + BIOS Minor Release
         /// </summary>
         HardwareID_1,

         /// <summary>
         /// HardwareID-2
         /// Manufacturer + Product Name + BIOS Vendor + BIOS Version + BIOS Major Release + BIOS Minor Release
         /// </summary>
         HardwareID_2,

         /// <summary>  
         /// HardwareID-3
         /// Manufacturer + Family + Product Name + SKU Number + Baseboard Manufacturer + Baseboard Product
         /// </summary>
         HardwareID_3,

         /// <summary>
         /// HardwareID-4
         /// Manufacturer + Family + Product Name + SKU Number
         /// </summary>
         HardwareID_4,

         /// <summary>
         /// HardwareID-5
         /// Manufacturer + Family + Product Name
         /// </summary>
         HardwareID_5,

         /// <summary>
         /// HardwareID-6
         /// Manufacturer + SKU Number + Baseboard Manufacturer + Baseboard Product
         /// </summary>
         HardwareID_6,

         /// <summary>
         /// HardwareID-7
         /// Manufacturer + SKU Number
         /// </summary>
         HardwareID_7,

         /// <summary>
         /// HardwareID-8
         /// Manufacturer + Product Name + Baseboard Manufacturer + Baseboard Product
         /// </summary>
         HardwareID_8,

         /// <summary>
         /// HardwareID-9
         /// <para></para>
         /// Manufacturer + Product Name
         /// </summary>
         HardwareID_9,

         /// <summary>
         /// HardwareID-10
         /// <para></para>
         /// Manufacturer + Family + Baseboard Manufacturer + Baseboard Product
         /// </summary>
         HardwareID_10,

         /// <summary>
         /// HardwareID-11
         /// Manufacturer + Family
         /// </summary>
         HardwareID_11,

         /// <summary>
         /// HardwareID-12
         /// Manufacturer + Enclosure Type
         /// </summary>
         HardwareID_12,

         /// <summary>
         /// HardwareID-13
         /// Manufacturer + Baseboard Manufacturer + Baseboard Product
         /// </summary>
         HardwareID_13,

         /// <summary>
         /// HardwareID-14
         /// Manufacturer
         /// </summary>
         HardwareID_14,
    }
}
