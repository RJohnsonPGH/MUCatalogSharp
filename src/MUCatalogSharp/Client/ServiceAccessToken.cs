// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UpdateServices.WebServices.ServerSync;

namespace MUCatalogSharp.Client;

/// <summary>
/// Grants access to an upstream update server. Requried for most requests to an update server.
/// </summary>
public sealed record ServiceAccessToken(IReadOnlyList<AuthPlugInInfo> AuthenticationInfo, 
    Microsoft.UpdateServices.WebServices.DssAuthentication.AuthorizationCookie AuthCookie,
	Cookie AccessCookie)
{
    /// <summary>
    /// Check if the access token will expire within the specified time span
    /// </summary>
    /// <param name="timeSpan">Time span from current time.</param>
    /// <returns>True if the token will expire before the timespan passes, false otherwise</returns>
    public bool ExpiresIn(TimeSpan timeSpan)
    {
        return AccessCookie == null || AccessCookie.Expiration < DateTime.Now.AddMinutes(timeSpan.TotalMinutes);
    }
}
