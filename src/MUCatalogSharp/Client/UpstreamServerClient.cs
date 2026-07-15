// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UpdateServices.WebServices.ServerSync;
using MUCatalogSharp.Metadata;
using MUCatalogSharp.Metadata.Content;
using MUCatalogSharp.Sources;
using System.Runtime.CompilerServices;
using System.Text;

namespace MUCatalogSharp.Client;

/// <summary>
/// <para>
/// Retrieves update metadata for expired updates from an upstream update server.
/// </para>
/// <para>
/// This class should only be used for retrieving individual expired updates when their ID is known. For querying updates use <see cref="UpstreamUpdatesSource"/>. 
/// For querying products and classifications, use <see cref="UpstreamSource"/>
/// </para>
/// </summary>
public sealed partial class UpstreamServerClient
{
    /// <summary>
    /// The account name used for authentication.
    /// </summary>
    public string AccountName { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The account GUID used for authentication.
    /// </summary>
    public Guid AccountGuid { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets the update server <see cref="Endpoint"/> this client connects to.
    /// </summary>
    /// <value>
    /// Update server <see cref="Endpoint"/>
    /// </value>
    public Endpoint UpstreamEndpoint { get; private set; }

    /// <summary>
    /// Client used to issue SOAP requests
    /// </summary>
    private readonly IServerSyncWebService ServerSyncClient;

    /// <summary>
    /// Cached access cookie. If not set in the constructor, a new access token will be obtained
    /// </summary>
    private ServiceAccessToken? _accessToken;

    /// <summary>
    /// Service configuration data. Contains maximum query limits, etc.
    /// If not passed to the constructor, this class will retrieve it from the service
    /// </summary>
    private ServerSyncConfigData? _configData;

    /// <summary>
    /// Initializes a new instance of UpstreamServerClient.
    /// </summary>
    /// <param name="upstreamEndpoint">The server endpoint this client will connect to.</param>
    public UpstreamServerClient(Endpoint upstreamEndpoint)
    {
        UpstreamEndpoint = upstreamEndpoint;

        var httpBindingWithTimeout = new System.ServiceModel.BasicHttpBinding()
        {
            ReceiveTimeout = new TimeSpan(0, 3, 0),
            SendTimeout = new TimeSpan(0, 3, 0),
            MaxBufferSize = int.MaxValue,
            ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
            MaxReceivedMessageSize = int.MaxValue,
            AllowCookies = true
        };

        var serviceEndpoint = new System.ServiceModel.EndpointAddress(UpstreamEndpoint.ServerSyncURI);
        if (serviceEndpoint.Uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
        {
            httpBindingWithTimeout.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
        }

        ServerSyncClient = new ServerSyncWebServiceClient(httpBindingWithTimeout, serviceEndpoint);
    }

	internal async Task<ServiceAccessToken> GetAccessTokenAsync()
    {
		if (_accessToken == null || _accessToken.ExpiresIn(TimeSpan.FromMinutes(2)))
		{
			var authenticator = new ClientAuthenticator(UpstreamEndpoint, AccountName, AccountGuid);
			_accessToken = await authenticator.AuthenticateAsync(_accessToken)
				.ConfigureAwait(false);
		}

        return _accessToken;
    }

    private async Task<ServerSyncConfigData> QueryConfigDataAsync()
    {
        if (_configData is not null)
        {
            return _configData;
        }

		var accessToken = await GetAccessTokenAsync()
	        .ConfigureAwait(false);

		var configDataRequest = new GetConfigDataRequest
        {
            GetConfigData = new GetConfigDataRequestBody()
            {
                configAnchor = null,
                cookie = accessToken.AccessCookie
            }
        };

        var configDataReply = await ServerSyncClient
            .GetConfigDataAsync(configDataRequest)
            .ConfigureAwait(false);
        if (configDataReply?.GetConfigDataResponse1?.GetConfigDataResult == null)
        {
            throw new Exception("Failed to get config data.");
        }

        _configData = configDataReply.GetConfigDataResponse1.GetConfigDataResult;
        return _configData;
    }

    internal async Task<IEnumerable<MicrosoftUpdatePackageIdentity>> GetCategoryIdsWithAnchorAsync(string oldAnchor)
    {
        if (string.IsNullOrEmpty(oldAnchor))
        {
            throw new InvalidOperationException("An anchor must be provided to get category IDs with anchor.");
        }

		var accessToken = await GetAccessTokenAsync()
	        .ConfigureAwait(false);

		// Create a request for categories
		var revisionIdRequest = new GetRevisionIdListRequest
        {
            GetRevisionIdList = new GetRevisionIdListRequestBody()
            {
                cookie = accessToken.AccessCookie,
                filter = new ServerSyncFilter()
            }
        };
        // GetConfig must be true to request just categories
        revisionIdRequest.GetRevisionIdList.filter.GetConfig = true;
        revisionIdRequest.GetRevisionIdList.filter.Anchor = oldAnchor;

        return await GetCategoryIdsWithAnchorAsync(revisionIdRequest)
            .ConfigureAwait(false);
    }

    internal async Task<IEnumerable<MicrosoftUpdatePackageIdentity>> GetCategoryIdsAsync()
    {
		var accessToken = await GetAccessTokenAsync()
	        .ConfigureAwait(false);

		await QueryConfigDataAsync()
            .ConfigureAwait(false);

        // Create a request for categories
        var revisionIdRequest = new GetRevisionIdListRequest
        {
            GetRevisionIdList = new GetRevisionIdListRequestBody()
            {
                cookie = accessToken.AccessCookie,
                filter = new ServerSyncFilter()
            }
        };
        // GetConfig must be true to request just categories
        revisionIdRequest.GetRevisionIdList.filter.GetConfig = true;

        return await GetCategoryIdsWithAnchorAsync(revisionIdRequest)
            .ConfigureAwait(false);
    }

    internal async Task<IEnumerable<MicrosoftUpdatePackageIdentity>> GetCategoryIdsWithAnchorAsync(GetRevisionIdListRequest revisionIdRequest)
    {
        var revisionsIdReply = await ServerSyncClient
            .GetRevisionIdListAsync(revisionIdRequest)
            .ConfigureAwait(false);
        if (revisionsIdReply?.GetRevisionIdListResponse1?.GetRevisionIdListResult == null)
        {
            throw new Exception("Failed to get revision ID list");
        }

        // Return IDs and the anchor for this query. The anchor can be used to get a delta list in the future.
        return revisionsIdReply
            .GetRevisionIdListResponse1
            .GetRevisionIdListResult
            .NewRevisions
            .Select(rawId => new MicrosoftUpdatePackageIdentity(rawId.UpdateID, rawId.RevisionNumber));
    }

    internal async Task<IEnumerable<MicrosoftUpdatePackageIdentity>> GetUpdateIdsAsync(UpstreamSourceFilter updatesFilter)
    {
		var accessToken = await GetAccessTokenAsync()
            .ConfigureAwait(false);

		// Create a request for categories
		var revisionIdRequest = new GetRevisionIdListRequest
        {
            GetRevisionIdList = new GetRevisionIdListRequestBody()
            {
                cookie = accessToken.AccessCookie,
                filter = updatesFilter.ToServerSyncFilter()
            }
        };

        // GetConfig must be false to request updates
        revisionIdRequest.GetRevisionIdList.filter.GetConfig = false;

        var revisionsIdReply = await ServerSyncClient
            .GetRevisionIdListAsync(revisionIdRequest)
            .ConfigureAwait(false);
		if (revisionsIdReply?.GetRevisionIdListResponse1?.GetRevisionIdListResult == null)
        {
            throw new Exception("Failed to get revision ID list");
        }

        // Return IDs and the anchor for this query. The anchor can be used to get a delta list in the future.
        return revisionsIdReply.GetRevisionIdListResponse1.GetRevisionIdListResult.NewRevisions.Select(
            rawId => new MicrosoftUpdatePackageIdentity(rawId.UpdateID, rawId.RevisionNumber));
    }

    /// <summary>
    /// Retrieves update data for the list of update ids
    /// </summary>
    /// <param name="updateIds">The ids to retrieve data for</param>
    internal async IAsyncEnumerable<MicrosoftUpdatePackage> GetUpdateDataForIdsAsync(MicrosoftUpdatePackageIdentity[] updateIds, [EnumeratorCancellation] CancellationToken cancelToken)
    {
	    cancelToken.ThrowIfCancellationRequested();

        var accessToken = await GetAccessTokenAsync()
			.ConfigureAwait(false);

		var configData = await QueryConfigDataAsync()
            .ConfigureAwait(false);

        var retrieveBatches = updateIds
            .Select(id => new UpdateIdentity() { UpdateID = id.Id, RevisionNumber = id.Revision })
            .Chunk(configData.MaxNumberOfUpdatesPerRequest);

        foreach(var batch in retrieveBatches)
        {
            cancelToken.ThrowIfCancellationRequested();

			var updateDataRequest = new GetUpdateDataRequest
            {
                GetUpdateData = new GetUpdateDataRequestBody()
                {
                    cookie = accessToken.AccessCookie,
                    updateIds = batch
                }
            };

            var updateDataReply = await ServerSyncClient
                .GetUpdateDataAsync(updateDataRequest)
                .ConfigureAwait(false);

            if (updateDataReply?.GetUpdateDataResponse1?.GetUpdateDataResult is null)
            {
				throw new Exception("Failed to get update data");
			}

            // Parse the list of files into a more usable format
            var fileUriDict = updateDataReply.GetUpdateDataResponse1.GetUpdateDataResult.fileUrls
                .Select(file => new UpdateFileUri(Convert.ToBase64String(file.FileDigest), file.MUUrl, file.UssUrl))
                .ToDictionary(file => file.Digest);

            foreach (var update in updateDataReply.GetUpdateDataResponse1.GetUpdateDataResult.updates)
            {
				byte[] metadata;
				if (!string.IsNullOrEmpty(update.XmlUpdateBlob))
				{
					metadata = Encoding.Unicode.GetBytes(update.XmlUpdateBlob);
				}
				else if (update.XmlUpdateBlobCompressed is not null && update.XmlUpdateBlobCompressed.Length > 0)
				{
					// This call will throw an exception if a decompressor is not available for the current platform.
					metadata = CabinetUtility.DecompressUnicodeData(update.XmlUpdateBlobCompressed);
				}
                else
                {
					throw new InvalidOperationException($"No XmlUpdateBlob or XmlUpdateBlobCompressed was provided for update: {update.Id}");
				}

				using var metadataStream = new MemoryStream(metadata, false);
				yield return MicrosoftUpdatePackage.FromMetadataXml(metadataStream, fileUriDict);
            }
        }
    }
}
