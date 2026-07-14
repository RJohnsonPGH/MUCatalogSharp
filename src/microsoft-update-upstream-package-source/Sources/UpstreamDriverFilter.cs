using Microsoft.UpdateServices.WebServices.ServerSync;
using System;
using System.Linq;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Source.Sources;

public sealed partial class UpstreamDriverFilter
{
	public required Guid[] ComputerFilter { get; init; }

	public required string[] PnpHardwareFilter { get; init; }

	internal ServerSyncDriverFilter ToServerSyncDriverFilter(string? anchor = null)
	{
		ServerSyncDriverFilter filter = new();

		if (ComputerFilter.Length > 0)
		{
			filter.ComputerIds = new IdAndDelta[ComputerFilter.Length];
			for (int i = 0; i < ComputerFilter.Length; i++)
			{
				filter.ComputerIds[i] = new IdAndDelta
				{
					// Request deltas if we have an anchor from a previous query
					Delta = !string.IsNullOrEmpty(anchor),
					Id = ComputerFilter[i]
				};
			}
		}

		if (PnpHardwareFilter.Length > 0)
		{
			filter.PnpHardwareIds = new HardwareIdAndDelta[PnpHardwareFilter.Length];
			for (int i = 0; i < PnpHardwareFilter.Length; i++)
			{
				filter.PnpHardwareIds[i] = new HardwareIdAndDelta
				{
					// Request deltas if we have an anchor from a previous query
					Delta = !string.IsNullOrEmpty(anchor),
					Id = PnpHardwareFilter[i]
				};
			}
		}

		filter.Anchor = anchor;

		return filter;
	}

	/// <summary>
	/// Override Equals for 2 DriverFilter objects
	/// </summary>
	/// <param name="obj">Other DriverFilter</param>
	/// <returns>
	/// <para>True if the two DriverFilter are identical (same computer and PnP hardware filters).</para>
	/// <para>False otherwise</para>
	/// </returns>
	public override bool Equals(object? obj)
	{
		if (obj is not UpstreamDriverFilter other)
		{
			return false;
		}

		if (ComputerFilter.Length != other.ComputerFilter.Length ||
			PnpHardwareFilter.Length != other.PnpHardwareFilter.Length)
		{
			return false;
		}

		return ComputerFilter.All(cat => other.ComputerFilter.Contains(cat))
			&& PnpHardwareFilter.All(cat => other.PnpHardwareFilter.Contains(cat));
	}

	/// <summary>
	/// Override equality operator DriverFilter objects
	/// </summary>
	/// <param name="lhs">Left DriverFilter</param>
	/// <param name="rhs">Right DriverFilter</param>
	/// <returns>
	/// <para>True if both lhs and rhs are DriverFilter and they contain the same computer and PnP hardware filters</para>
	/// <para>False otherwise</para>
	/// </returns>
	public static bool operator ==(UpstreamDriverFilter lhs, UpstreamDriverFilter rhs)
	{
		if (lhs is null)
		{
			return rhs is null;
		}
		else
		{
			return lhs.Equals(rhs);
		}
	}

	/// <summary>
	/// Override inequality operator DriverFilter objects
	/// </summary>
	/// <param name="lhs">Left DriverFilter</param>
	/// <param name="rhs">Right DriverFilter</param>
	/// <returns>
	/// <para>True if both lhs and rhs are not DriverFilter or they contain different computer and PnP hardware filters</para>
	/// <para>False otherwise</para>
	/// </returns>
	public static bool operator !=(UpstreamDriverFilter lhs, UpstreamDriverFilter rhs)
	{
		return !(lhs == rhs);
	}

	/// <summary>
	/// Returns a hash code based on the hash codes of the contained computer and PnP hardware filters
	/// </summary>
	/// <returns>Hash code</returns>
	public override int GetHashCode()
	{
		int hash = 0;
		ComputerFilter.ToList().ForEach(cat => hash |= cat.GetHashCode());
		PnpHardwareFilter.ToList().ForEach(cat => hash |= cat.GetHashCode());

		return hash;
	}
}
