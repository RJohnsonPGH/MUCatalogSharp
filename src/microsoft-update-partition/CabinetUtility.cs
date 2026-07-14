// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace Microsoft.PackageGraph.MicrosoftUpdate;

/// <summary>
/// Performs CAB compression and decompression. On Linux, it requires the cabextract; on windows it requires expand.exe
/// </summary>
public sealed class CabinetUtility
{
    /// <summary>
    /// Recompress the given bytes
    /// </summary>
    /// <param name="compressedData">Data to compress</param>
    /// <returns>Compressed data</returns>
    /// <exception cref="NotImplementedException">If not implemented on the current platform</exception>
    public static byte[] DecompressUnicodeData(byte[] compressedData)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return DecompressUnicodeDataWindows(compressedData);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return DecompressUnicodeDataLinux(compressedData);
        }
        else
        {
            throw new NotImplementedException("No decompressor available for the current platform");
        }
    }

    private static byte[] DecompressUnicodeDataWindows(byte[] compressedData)
    {
		// We use temporary files to write the in-memory cabinet,
		// run expand on it then read the resulting file back in memory
		var cabTempFile = Path.GetTempFileName();
        var xmlTempFile = Path.GetTempFileName();

        byte[] returnBytes;

        File.WriteAllBytes(cabTempFile, compressedData);

        var startInfo = new ProcessStartInfo("expand.exe", $"\"{cabTempFile}\" \"{xmlTempFile}\"")
        {
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var expandProcess = Process.Start(startInfo) 
            ?? throw new InvalidOperationException("Failed to start expand process.");
        expandProcess.WaitForExit();

		returnBytes = File.ReadAllBytes(xmlTempFile);

        if (File.Exists(cabTempFile))
        {
            File.Delete(cabTempFile);
        }

        if (File.Exists(xmlTempFile))
        {
            File.Delete(xmlTempFile);
        }

        return returnBytes;
    }

    private static byte[] DecompressUnicodeDataLinux(byte[] compressedData)
    {
        // We use temporary files to write the in-memory cabinet,
        // Then run cabextract on it with --pipe output
        var cabTempFile = Path.GetTempFileName();
			byte[] returnBytes;

        File.WriteAllBytes(cabTempFile, compressedData);

        var startInfo = new ProcessStartInfo("cabextract", $"--pipe \"{cabTempFile}\"")
        {
            UseShellExecute = false,
            // The decompressed text is Unicode
            StandardOutputEncoding = Encoding.Unicode,
            RedirectStandardOutput = true
        };

        var expandProcess = Process.Start(startInfo) ??
            throw new InvalidOperationException("Failed to start cabextract process.");

        // Read the decompressed data from the pipe
        // Discard standard error output
        expandProcess.StandardOutput.ReadToEnd();
        expandProcess.WaitForExit();

		// Recompress the XML with GZIP as UTF8
		returnBytes = File.ReadAllBytes(cabTempFile);

		if (File.Exists(cabTempFile))
		{
			File.Delete(cabTempFile);
		}
		return returnBytes;
    }
}
