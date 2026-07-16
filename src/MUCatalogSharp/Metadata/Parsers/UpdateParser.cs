using MUCatalogSharp.Metadata.Content;
using MUCatalogSharp.Metadata.Parsers.Interfaces;
using MUCatalogSharp.Metadata.Update;
using MUCatalogSharp.Metadata.Update.Files;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace MUCatalogSharp.Metadata.Parsers;

/// <summary>
/// Provides methods for parsing Microsoft Update metadata from XML documents.
/// </summary>
public static class UpdateParser
{
	/// <summary>
	/// Creates an instance of type <typeparamref name="T"/> from the provided <paramref name="navigator"/> and <paramref name="manager"/>.
	/// </summary>
	/// <typeparam name="T">The type of the object to create.</typeparam>
	/// <param name="navigator">The XPathNavigator positioned at the XML node to parse.</param>
	/// <param name="manager">The XmlNamespaceManager for resolving namespaces in the XML document.</param>
	/// <returns>An instance of type <typeparamref name="T"/> if the node is found; otherwise, null.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the navigator is not positioned at a valid parent node for the specified type.</exception>
	internal static T? CreateOptional<T>(XPathNavigator navigator, XmlNamespaceManager manager) where T : class, ICreatable<T>
	{
		ArgumentNullException.ThrowIfNull(navigator);
		ArgumentNullException.ThrowIfNull(manager);

		// Validate that the XML navigator is at one of the expected nodes
		if (!T.ValidParentNodes.Contains(navigator.Name, StringComparer.InvariantCultureIgnoreCase))
		{
			throw new InvalidOperationException($"Expected navigator at one of [{string.Join(", ", T.ValidParentNodes)}], but found '{navigator.Name}'");
		}

		var movedNavigator = navigator.SelectSingleNode(T.XPathQuery, manager);
		if (movedNavigator is null)
		{
			return null;
		}

		return T.Create(movedNavigator, manager);
	}

	/// <summary>
	/// Creates an instance of type <typeparamref name="T"/> from the provided <paramref name="navigator"/> and <paramref name="manager"/>.
	/// </summary>
	/// <typeparam name="T">The type of the object to create.</typeparam>
	/// <param name="navigator">The XPathNavigator positioned at the XML node to parse.</param>
	/// <param name="manager">The XmlNamespaceManager for resolving namespaces in the XML document.</param>
	/// <returns>An instance of type <typeparamref name="T"/> if the node is found; otherwise, null.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the navigator is not positioned at a valid parent node for the specified type.</exception>
	internal static T Create<T>(XPathNavigator navigator, XmlNamespaceManager manager) where T : class, ICreatable<T>
    {
        ArgumentNullException.ThrowIfNull(navigator);
        ArgumentNullException.ThrowIfNull(manager);

        // Validate that the XML navigator is at one of the expected nodes
        if (!T.ValidParentNodes.Contains(navigator.Name, StringComparer.InvariantCultureIgnoreCase))
        {
            throw new InvalidOperationException($"Expected navigator at one of [{string.Join(", ", T.ValidParentNodes)}], but found '{navigator.Name}'");
        }

        // Move the navigator to the appropriate node
        var movedNavigator = navigator.SelectSingleNode(T.XPathQuery, manager) ??
            throw new InvalidOperationException($"Failed to select the node '{T.XPathQuery}'");

        return T.Create(movedNavigator, manager);
    }

	/// <summary>
	/// Creates a collection of instances of type <typeparamref name="T"/> from the provided <paramref name="navigator"/> and <paramref name="manager"/>.
	/// </summary>
	/// <typeparam name="T">The type of the objects to create.</typeparam>
	/// <param name="navigator">The XPathNavigator positioned at the XML node to parse.</param>
	/// <param name="manager">The XmlNamespaceManager for resolving namespaces in the XML document.</param>
	/// <returns>A read-only list of instances of type <typeparamref name="T"/>.</returns>
	/// <exception cref="InvalidOperationException">Thrown if a node cannot be selected or created.</exception>
	internal static IReadOnlyList<T> CreateCollection<T>(XPathNavigator navigator, XmlNamespaceManager manager) where T : ICreatable<T>, IMetadataCollection<T>
	{
		ArgumentNullException.ThrowIfNull(navigator);
		ArgumentNullException.ThrowIfNull(manager);

		var list = new List<T>();
		var nodes = navigator.Select(T.XPathQuery, manager);
		while (nodes.MoveNext())
		{
			if (nodes.Current is null)
			{
				throw new InvalidOperationException($"Failed to select the node '{T.XPathQuery}'");
			}

			list.Add(T.Create(nodes.Current.Clone(), manager));
		}
		return list;
	}

    /// <summary>
    /// Creates an instance of type <typeparamref name="T"/> from the provided <paramref name="navigator"/> and <paramref name="manager"/> using the specified XPath query.
    /// </summary>
    /// <typeparam name="T">The type of the property to create.</typeparam>
    /// <param name="navigator">The XPathNavigator positioned at the XML node to parse.</param>
    /// <param name="manager">The XmlNamespaceManager for resolving namespaces in the XML document.</param>
    /// <param name="xPathQuery">The XPath query to select the node.</param>
    /// <returns>An instance of type <typeparamref name="T"/> if the node is found; otherwise, throws an exception.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the required property is not found or empty.</exception>
    public static T CreateProperty<T>(XPathNavigator navigator, XmlNamespaceManager manager, string xPathQuery) where T : IParsable<T>
    {
        return TryCreateOptionalProperty<T>(navigator, manager, xPathQuery, out var result) ? result
            : throw new InvalidOperationException($"Required property not found or empty for XPath query: {xPathQuery}");
    }

	/// <summary>
	/// Tries to create an instance of type <typeparamref name="T"/> from the provided <paramref name="navigator"/> and <paramref name="manager"/> using the specified XPath query.
	/// </summary>
	/// <typeparam name="T">The type of the property to create.</typeparam>
	/// <param name="navigator">The XPathNavigator positioned at the XML node to parse.</param>
	/// <param name="manager">The XmlNamespaceManager for resolving namespaces in the XML document.</param>
	/// <param name="xPathQuery">The XPath query to select the node.</param>
	/// <param name="property">The created property if successful; otherwise, default.</param>
	/// <returns>True if the property was successfully created; otherwise, false.</returns>
	public static bool TryCreateOptionalProperty<T>(XPathNavigator navigator, XmlNamespaceManager manager, string xPathQuery, [NotNullWhen(true)] out T? property) where T : IParsable<T>
	{
		ArgumentNullException.ThrowIfNull(navigator);
		ArgumentNullException.ThrowIfNull(manager);

		XPathExpression query = navigator.Compile(xPathQuery);
		query.SetContext(manager);

		if (navigator.Evaluate(query) is not XPathNodeIterator result)
		{
			throw new InvalidOperationException($"Failed to evaluate XPath query: {xPathQuery}");
		}

		if (result.Count <= 0)
		{
			property = default;
			return false;
		}

		result.MoveNext();

		// Check for null
		if (result.Current is null)
		{
			property = default;
			return false;
		}

		property = T.Parse(result.Current.Value, CultureInfo.InvariantCulture);
		return true;
	}

	/// <summary>
	/// Resolves a collection of <see cref="UpdateFile"/> instances from the provided <paramref name="metadataNavigator"/>, <paramref name="namespaceManager"/>, and <paramref name="fileUris"/>.
	/// </summary>
	/// <param name="metadataNavigator">The XPathNavigator positioned at the XML node to parse.</param>
	/// <param name="namespaceManager">The XmlNamespaceManager for resolving namespaces in the XML document.</param>
	/// <param name="fileUris">A dictionary mapping file digests to their corresponding URIs.</param>
	/// <returns>A collection of <see cref="UpdateFile"/> instances if successful; otherwise, null.</returns>
	public static IReadOnlyCollection<UpdateFile>? ResolveFiles(
		XPathNavigator metadataNavigator,
		XmlNamespaceManager namespaceManager,
		IReadOnlyDictionary<string, UpdateFileUri> fileUris) =>
		UpdateParser.CreateOptional<UpdateFilesCollection>(metadataNavigator, namespaceManager) switch
		{
			null => null,
			UpdateFilesCollection unresolvedFiles => [.. unresolvedFiles.Select(unresolvedFile => new UpdateFile(
					Digest: unresolvedFile.Digest,
					DigestAlgorithm: unresolvedFile.DigestAlgorithm,
					Uri: ResolveUri(fileUris, unresolvedFile.Digest),
					FileName: unresolvedFile.FileName,
					Size: unresolvedFile.Size,
					Modified: unresolvedFile.Modified,
					PatchingType: unresolvedFile.PatchingType,
					AdditionalDigests: unresolvedFile.AdditionalDigests
				)
			)]
		};

	/// <summary>
	/// Resolves the URI for a given file digest using the provided dictionary of file URIs, prefers the Upstream Server URI if available, otherwise falls back to the Microsoft Update URI.
	/// </summary>
	/// <param name="fileUris">A dictionary mapping file digests to their corresponding URIs.</param>
	/// <param name="digest">The digest of the file for which to resolve the URI.</param>
	/// <returns>The resolved URI for the given file digest.</returns>
	/// <exception cref="KeyNotFoundException">Thrown if no URI is found for the given digest.</exception>
	/// <exception cref="InvalidOperationException">Thrown if no valid URI is found for the given digest.</exception>
	private static Uri ResolveUri(IReadOnlyDictionary<string, UpdateFileUri> fileUris, string digest)
	{
		if (!fileUris.TryGetValue(digest, out var fileUri))
		{
			throw new KeyNotFoundException($"No URI found for digest: {digest}");
		}

		if (Uri.TryCreate(fileUri.UssUri, UriKind.Absolute, out var ussUri))
		{
			return ussUri;
		}

		if (Uri.TryCreate(fileUri.MuUri, UriKind.Absolute, out var muUri))
		{
			return muUri;
		}

		throw new InvalidOperationException($"No valid URI found for digest: {digest}");
	}
}
