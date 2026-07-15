// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace MUCatalogSharp.Metadata;

/// <summary>
/// Represents the identity of a Microsoft Update package (update)
/// </summary>
public record MicrosoftUpdatePackageIdentity(Guid Id, int Revision) : IIdentity
{
    public int CompareTo(object? obj)
    {
        if (obj is not MicrosoftUpdatePackageIdentity other)
        {
            return -1;
        }

        int idComparison = Id.CompareTo(other.Id);
        if (idComparison != 0)
        {
            return idComparison;
        }

        return Revision.CompareTo(other.Revision);
    }

    public override int GetHashCode() => HashCode.Combine(Id, Revision);
}
