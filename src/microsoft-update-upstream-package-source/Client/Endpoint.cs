// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PackageGraph.MicrosoftUpdate.Source.Client;

/// <summary>
/// Identifies an Upstream Update Server.
/// <para>
/// Use <see cref="Default"/> to get the endpoint of the official Microsoft Upstream Update Server.
/// </para>
/// </summary>
/// <remarks>
/// Initializes a new instance of the Endpoint class, with the specified URI to the upstream update server
/// </remarks>
/// <param name="uri">Absolute URI of the upstream update server</param>
public class Endpoint(string uri)
{
    /// <summary>
    /// The Microsoft upstream server root address. Used to construct the default endpoint.
    /// </summary>
    private const string MicrosoftUpstreamRoot = @"https://sws.update.microsoft.com";

    /// <summary>
    /// Gets the absolute URI of the upstream server.
    /// </summary>
    /// <value>
    /// Absolute URI string to upstream server.
    /// </value>
    public readonly string URI = uri;

    /// <summary>
    /// Gets the absolute URI of the server-to-server sync webservice.
    /// </summary>
    /// <value>
    /// Absolute URI string to server-server sync webservice.
    /// </value>
    internal string ServerSyncURI => URI + @"/ServerSyncWebService/ServerSyncWebService.asmx";

    /// <summary>
    /// Gets the endpoint of the official Microsoft upstream update server
    /// </summary>
    /// <value>
    /// Upstream update server endpoint
    /// </value>
    public static Endpoint Default => new(MicrosoftUpstreamRoot);

    /// <summary>
    /// Creates a complete URL to a DSS authentication web service based on the upstream URL and the DSS relative URL
    /// </summary>
    /// <param name="serviceRelativeUrl">The DSS service URL (relative)</param>
    /// <returns>The complete URL to the DSS authentication endpoint</returns>
    internal string GetAuthenticationEndpointFromRelativeUrl(string serviceRelativeUrl)
    {
        return URI + "/" + serviceRelativeUrl;
    }
}
