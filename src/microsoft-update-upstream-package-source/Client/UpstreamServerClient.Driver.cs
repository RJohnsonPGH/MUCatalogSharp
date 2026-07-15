using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using Microsoft.PackageGraph.MicrosoftUpdate.Source.Sources;
using Microsoft.UpdateServices.WebServices.ServerSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Source.Client;

public sealed partial class UpstreamServerClient
{
	/// <summary>
	/// Gets a list of driver IDs that match the specified filter.
	/// </summary>
	/// <remarks>
	/// At least one Computer Id and Pnp Hardware Id must be specified in the filter. The WSUSSS server will not return any drivers if the filter is empty.
	/// </remarks>
	/// <param name="filter">The filter to apply when retrieving driver IDs.</param>
	/// <returns>A list of driver IDs that match the specified filter.</returns>
	internal async Task<IEnumerable<MicrosoftUpdatePackageIdentity>> GetDriverIdsAsync(UpstreamDriverFilter filter)
	{
		var accessToken = await GetAccessTokenAsync()
			.ConfigureAwait(false);

		await QueryConfigDataAsync()
			.ConfigureAwait(false);

		// Create a request for drivers
		var driverIdRequest = new GetDriverIdListRequest
		{
			GetDriverIdList = new GetDriverIdListRequestBody()
			{
				cookie = accessToken.AccessCookie,
				filter = filter.ToServerSyncDriverFilter()
			}
		};

		return await GetDriverIdsWithAnchorAsync(driverIdRequest)
			.ConfigureAwait(false);
	}

	internal async Task<IEnumerable<MicrosoftUpdatePackageIdentity>> GetDriverIdsWithAnchorAsync(GetDriverIdListRequest driverIdRequest)
	{
		var driverIdReply = await ServerSyncClient
			.GetDriverIdListAsync(driverIdRequest)
			.ConfigureAwait(false);
		if (driverIdReply?.GetDriverIdListResponse1?.GetDriverIdListResult == null)
		{
			throw new Exception("Failed to get driver ID list");
		}

		// Return IDs and the anchor for this query. The anchor can be used to get a delta list in the future.
		return driverIdReply
			.GetDriverIdListResponse1
			.GetDriverIdListResult
			.NewRevisions
			.Select(rawId => new MicrosoftUpdatePackageIdentity(rawId.UpdateID, rawId.RevisionNumber));
	}
}
