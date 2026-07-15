// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UpdateServices.WebServices.ServerSync;

namespace MUCatalogSharp.Sources;


public sealed class UpstreamSourceFilter
{
    /// <summary>
    /// Gets the list of products allowed by the filter.
    /// If this list if empty, no updates will match the filter. Add product IDs to this list to have them match the filter.
    /// </summary>
    /// <value>List of product identities.</value>
    public required Guid[] ProductsFilter { get; init; }

    /// <summary>
    /// Gets the list of classifications allowed by the filter.
    /// If this list if empty, no updates will match the filter. Add classification IDs to this list to have them match the filter.
    /// </summary>
    /// <value>List of classification identities.</value>
    public required Guid[] ClassificationsFilter { get; init; }

    /// <summary>
    /// Creates a ServerSyncFilter object to be used with GetRevisionIdListAsync
    /// </summary>
    /// <returns>A ServerSyncFilter instance</returns>
    internal ServerSyncFilter ToServerSyncFilter(string? anchor = null)
    {
        ServerSyncFilter filter = new();

        if (ProductsFilter.Length > 0)
        {
            filter.Categories = new IdAndDelta[ProductsFilter.Length];
            for (int i = 0; i < ProductsFilter.Length; i++)
            {
                filter.Categories[i] = new IdAndDelta
                {
                    // Request deltas if we have an anchor from a previous query
                    Delta = !string.IsNullOrEmpty(anchor),
                    Id = ProductsFilter[i]
                };
            }
        }

        if (ClassificationsFilter.Length > 0)
        {
            filter.Classifications = new IdAndDelta[ClassificationsFilter.Length];
            for (int i = 0; i < ClassificationsFilter.Length; i++)
            {
                filter.Classifications[i] = new IdAndDelta
                {
                    // Request deltas if we have an anchor from a previous query
                    Delta = !string.IsNullOrEmpty(anchor),
                    Id = ClassificationsFilter[i]
                };
            }
        }

        filter.Anchor = anchor;

        return filter;
    }

    /// <summary>
    /// Override Equals for 2 SourceFilter objects
    /// </summary>
    /// <param name="obj">Other SourceFilter</param>
    /// <returns>
    /// <para>True if the two SourceFilter are identical (same product and classification filters).</para>
    /// <para>False otherwise</para>
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is not UpstreamSourceFilter other)
        {
            return false;
        }

        if (ProductsFilter.Length != other.ProductsFilter.Length ||
            ClassificationsFilter.Length != other.ClassificationsFilter.Length)
        {
            return false;
        }

        return ProductsFilter.All(cat => other.ProductsFilter.Contains(cat))
            && ClassificationsFilter.All(cat => other.ClassificationsFilter.Contains(cat));
    }

    /// <summary>
    /// Override equality operator SourceFilter objects
    /// </summary>
    /// <param name="lhs">Left SourceFilter</param>
    /// <param name="rhs">Right SourceFilter</param>
    /// <returns>
    /// <para>True if both lhs and rhs are SourceFilter and they contain the same classification and product filters</para>
    /// <para>False otherwise</para>
    /// </returns>
    public static bool operator ==(UpstreamSourceFilter lhs, UpstreamSourceFilter rhs)
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
    /// Override inequality operator SourceFilter objects
    /// </summary>
    /// <param name="lhs">Left SourceFilter</param>
    /// <param name="rhs">Right SourceFilter</param>
    /// <returns>
    /// <para>True if both lhs and rhs are not QueryFilter or they contain different classification and product filters</para>
    /// <para>False otherwise</para>
    /// </returns>
    public static bool operator !=(UpstreamSourceFilter lhs, UpstreamSourceFilter rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    /// Returns a hash code based on the hash codes of the contained classification and products
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        int hash = 0;
        ProductsFilter.ToList().ForEach(cat => hash |= cat.GetHashCode());
        ClassificationsFilter.ToList().ForEach(cat => hash |= cat.GetHashCode());

        return hash;
    }
}
