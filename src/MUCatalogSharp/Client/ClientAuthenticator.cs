// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UpdateServices.WebServices.DssAuthentication;
using Microsoft.UpdateServices.WebServices.ServerSync;

namespace MUCatalogSharp.Client;

internal sealed class ClientAuthenticator(Endpoint endpoint, string accountName, Guid accountGuid)
{
    /// <summary>
    /// Performs authentication with an upstream update server, using a previously issued service access token.
    /// </summary>
    /// <remarks>
    /// Refreshing an old token with this method is faster than obtaining a new token as it requires fewer server roundtrips.
    /// 
    /// If the access cookie does not expire within 30 minutes, the function succeeds and the old token is returned.
    /// </remarks>
    /// <param name="cachedAccessToken">The previously issued access token.</param>
    /// <returns>The new ServiceAccessToken</returns>
    internal async Task<ServiceAccessToken> AuthenticateAsync(ServiceAccessToken? cachedAccessToken)
    {
        if (cachedAccessToken == null)
        {
            return await AuthenticateAsync();
        }

        // Check if the cached access cookie expires in the next 30 minutes; if not, return the new token
        // with this cookie
        if (!cachedAccessToken.ExpiresIn(TimeSpan.FromMinutes(30)))
        {
            return cachedAccessToken;
        }
  
        // Get a new access cookie
        try
        {
            return cachedAccessToken with { AccessCookie = await GetServerAccessCookieAsync(cachedAccessToken.AuthCookie) };
		}
        catch (UpstreamServerException ex)
        {
            if (ex.ErrorCode == UpstreamServerErrorCode.InvalidAuthorizationCookie)
            {
                return await AuthenticateAsync();
            }
            else
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Performs authentication with an upstream update service.
    /// </summary>
    /// <returns>A new access token.</returns>
    private async Task<ServiceAccessToken> AuthenticateAsync()
    {
		IReadOnlyList<AuthPlugInInfo> authenticationInfo = [.. await GetAuthenticationInfoAsync()
                .ConfigureAwait(false)];

        var authCookie = await GetAuthorizationCookieAsync(authenticationInfo[0])
            .ConfigureAwait(false);
        var accessCookie = await GetServerAccessCookieAsync(authCookie)
            .ConfigureAwait(false);

        return new ServiceAccessToken(authenticationInfo, authCookie, accessCookie);
    }

    /// <summary>
    /// Retrieves authentication information from a WSUS server.
    /// </summary>
    /// <returns>List of supported authentication methods</returns>
    private async Task<AuthPlugInInfo[]> GetAuthenticationInfoAsync()
    {
        GetAuthConfigResponse authConfigResponse;

        var httpBinding = new System.ServiceModel.BasicHttpBinding();
        var upstreamEndpoint = new System.ServiceModel.EndpointAddress(endpoint.ServerSyncURI);
        if (upstreamEndpoint.Uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
        {
            httpBinding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
        }

        // Create a WSUS server sync client
        IServerSyncWebService serverSyncClient = new ServerSyncWebServiceClient(httpBinding, upstreamEndpoint);

        // Retrieve the authentication information
        authConfigResponse = await serverSyncClient
            .GetAuthConfigAsync(new GetAuthConfigRequest())
            .ConfigureAwait(false);

        if (authConfigResponse == null)
        {
            throw new Exception("Authentication config response was null.");
        }
        else if (authConfigResponse.GetAuthConfigResponse1.GetAuthConfigResult.AuthInfo == null)
        {
            throw new Exception("Authentication config payload was null.");
        }

        return authConfigResponse.GetAuthConfigResponse1.GetAuthConfigResult.AuthInfo;
    }

    /// <summary>
    /// Retrieves an authentication cookie from a DSS service.
    /// </summary>
    /// <returns>An authentication cookie</returns>
    private async Task<Microsoft.UpdateServices.WebServices.DssAuthentication.AuthorizationCookie> GetAuthorizationCookieAsync(AuthPlugInInfo authInfo)
    {
        var httpBinding = new System.ServiceModel.BasicHttpBinding();
        var upstreamEndpoint = new System.ServiceModel.EndpointAddress(endpoint.GetAuthenticationEndpointFromRelativeUrl(authInfo.ServiceUrl));

        if (upstreamEndpoint.Uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
        {
            httpBinding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
        }

        // Create a DSS client using the endpoint retrieved above
        IDSSAuthWebService authenticationService = new DSSAuthWebServiceClient(httpBinding, upstreamEndpoint);

        // Issue the request. All accounts are allowed, so we just generate a random account guid and name
        var cookieRequest = new GetAuthorizationCookieRequest
        {
            GetAuthorizationCookie = new GetAuthorizationCookieRequestBody
            {
                accountGuid = accountName,
                accountName = accountGuid.ToString()
            }
        };

        var getAuthCookieResponse = await authenticationService
            .GetAuthorizationCookieAsync(cookieRequest)
            .ConfigureAwait(false);

        if (getAuthCookieResponse == null ||
            getAuthCookieResponse.GetAuthorizationCookieResponse1.GetAuthorizationCookieResult.CookieData == null)
        {
            throw new Exception("Failed to get authorization token. Response or cookie is null.");
        }

        return getAuthCookieResponse.GetAuthorizationCookieResponse1.GetAuthorizationCookieResult;
    }

    /// <summary>
    /// Retrieves a server access cookie based on an authentication cookie.
    /// </summary>
    /// <param name="authCookie">The auth cookie to use when requesting the access cookie</param>
    /// <returns>An access cookie</returns>
    private async Task<Cookie> GetServerAccessCookieAsync(Microsoft.UpdateServices.WebServices.DssAuthentication.AuthorizationCookie authCookie)
    {
        var httpBinding = new System.ServiceModel.BasicHttpBinding();
        var upstreamEndpoint = new System.ServiceModel.EndpointAddress(endpoint.ServerSyncURI);
        if (upstreamEndpoint.Uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
        {
            httpBinding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
        }

        // Create a service client on the default Microsoft upstream server.
        IServerSyncWebService serverSyncClient = new ServerSyncWebServiceClient(httpBinding, upstreamEndpoint);

        // Create an access cookie request using the authentication cookie parameter.
        var cookieRequest = new GetCookieRequest
        {
            GetCookie = new GetCookieRequestBody()
            {
                authCookies =
                [
                    new Microsoft.UpdateServices.WebServices.ServerSync.AuthorizationCookie()
                    {
                        CookieData = authCookie.CookieData,
                        PlugInId = authCookie.PlugInId
                    }
                ],
                oldCookie = null,
                protocolVersion = "1.7"
            }
        };

        cookieRequest.GetCookie.authCookies = [new Microsoft.UpdateServices.WebServices.ServerSync.AuthorizationCookie()];
        cookieRequest.GetCookie.authCookies[0].CookieData = authCookie.CookieData;
        cookieRequest.GetCookie.authCookies[0].PlugInId = authCookie.PlugInId;
        cookieRequest.GetCookie.oldCookie = null;
        cookieRequest.GetCookie.protocolVersion = "1.7";

        GetCookieResponse cookieResponse;
        try
        {
            cookieResponse = await serverSyncClient
                .GetCookieAsync(cookieRequest)
                .ConfigureAwait(false);
        }
        catch(System.ServiceModel.FaultException ex)
        {
            throw new UpstreamServerException(ex);
        }

        if (cookieResponse == null ||
            cookieResponse.GetCookieResponse1.GetCookieResult.EncryptedData == null)
        {
            throw new Exception("Failed to get access cookie. Response or cookie is null.");
        }

        return cookieResponse.GetCookieResponse1.GetCookieResult;
    }
}
