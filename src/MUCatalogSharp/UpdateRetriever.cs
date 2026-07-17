using MUCatalogSharp.Client;
using MUCatalogSharp.Filters;
using MUCatalogSharp.Metadata;
using MUCatalogSharp.Metadata.Categories;
using MUCatalogSharp.Metadata.Update;
using MUCatalogSharp.Models;
using MUCatalogSharp.Progress;
using MUCatalogSharp.Sources;
using System.Runtime.CompilerServices;

namespace MUCatalogSharp;

/// <summary>
/// Provides methods to retrieve categories and updates from the Microsoft Update upstream source, with support for filtering and progress reporting.
/// </summary>
public static class UpdateRetriever
{
    /// <summary>
    /// Retrieves categories from the Microsoft Update upstream source asynchronously.
    /// </summary>
    /// <param name="progress">The progress reporter for detailed progress updates.</param>
    /// <param name="categoryFilter">The filter to apply to the categories.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>An asynchronous stream of categories.</returns>
    public static async IAsyncEnumerable<Category> GetCategoriesAsync(IProgress<DetailedProgress> progress, 
        CategoryTypeFilter categoryFilter = CategoryTypeFilter.Classification | CategoryTypeFilter.Product, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        UpstreamSource categoriesSource = new(Endpoint.Default);

        var detailedProgress = new DetailedProgress("Retrieving categories", 0, 0);
        var internalProgress = new Progress<(int, int)>(x =>
		{
			var (Current, Total) = x;
			progress.Report(detailedProgress with { Count = Current, Total = Total });
		});

        await foreach (var currentClassification in categoriesSource
			.GetCategoriesAsync(internalProgress, cancellationToken: cancellationToken)
			.ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Category? category = currentClassification switch
            {
                ClassificationCategory when categoryFilter.HasFlag(CategoryTypeFilter.Classification) => new Classification()
                {
                    Id = currentClassification.Id.Id,
                    Name = GetLocalizedProperties(currentClassification).Title,
				},
                ProductCategory when categoryFilter.HasFlag(CategoryTypeFilter.Product) => new Product()
                {
                    Id = currentClassification.Id.Id,
                    Revision = currentClassification.Id.Revision,
                    Name = GetLocalizedProperties(currentClassification).Title,
					Categories = [] // Used to build the product tree (e.g. Microsoft -> Windows -> Windows 10)
				},
                DetectoidCategory when categoryFilter.HasFlag(CategoryTypeFilter.Detectoid) => new Detectoid()
                {
                    Id = currentClassification.Id.Id,
                    Revision = currentClassification.Id.Revision,
                    Name = GetLocalizedProperties(currentClassification).Title,
				},
                _ => null
            };

            if (category is not null)
            {
                yield return category;
            }
        }
    }

	/// <summary>
	/// Retrieves updates from the Microsoft Update upstream source asynchronously, filtered by product and classification.
	/// </summary>
	/// <param name="progress">The progress reporter for detailed progress updates.</param>
	/// <param name="productFilter">The filter to apply to the products.</param>
	/// <param name="classificationFilter">The filter to apply to the classifications.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>A task representing the asynchronous operation, with a list of updates as the result.</returns>
	/// <exception cref="ArgumentException">Thrown if no products or classifications are provided.</exception>
	/// <exception cref="NotSupportedException">Thrown if an update type is not supported.</exception>
	/// <exception cref="InvalidOperationException">Thrown if an update cannot be processed.</exception>
	public static async Task<IReadOnlyList<IUpdate>> GetUpdatesAsync(IProgress<DetailedProgress> progress, 
        Guid[] productFilter,
        Guid[] classificationFilter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(productFilter, nameof(productFilter));
		ArgumentNullException.ThrowIfNull(classificationFilter, nameof(classificationFilter));
		if (productFilter.Length == 0 || classificationFilter.Length == 0)
		{
			throw new ArgumentException("At least one product and one classification filter must be provided.");
		}

		cancellationToken.ThrowIfCancellationRequested();

		// Get categories for resolution
		List<Category> categories = [];
		await foreach (var current in GetCategoriesAsync(progress, cancellationToken: cancellationToken)
			.ConfigureAwait(false))
		{
			categories.Add(current);
		}

		UpstreamSource updatesSource = new(Endpoint.Default);
        var detailedProgress = new DetailedProgress("Retrieving updates", 0, 0);
		var internalProgress = new Progress<(int, int)>(x =>
        {
            var (Current, Total) = x;
			progress.Report(detailedProgress with { Count = Current, Total = Total });
		});

		// Filter so only the particular types and products wanted will be downloaded and processed
		var updatesFilter = new UpstreamSourceFilter()
        { 
            ClassificationsFilter = classificationFilter ?? [], 
            ProductsFilter = productFilter ?? []
        };

        // Iterate through all of the metadata returned by the upstream source
		List<IUpdate> updates = [];
		await foreach (var current in updatesSource
            .GetUpdatesAsync(internalProgress, updatesFilter, cancellationToken: cancellationToken)
            .ConfigureAwait(false))
        {
			var localizedProperties = GetLocalizedProperties(current);
			updates.Add(current switch
			{
				SoftwareUpdate softwareUpdate => CreateSoftwareUpdate(softwareUpdate, categories, localizedProperties),
				DriverUpdate driverUpdate => CreateDriverUpdate(driverUpdate, categories, localizedProperties),
				_ => throw new NotSupportedException($"Update type {current.GetType().Name} is not supported.")
			});
        }

		// Resolve all bundled updates
		// Create a dictionary for fast lookup of updates by ID
		var updateDictionary = updates.ToDictionary(x => x switch
		{
			Update update => update.Id,
			Driver driver => driver.Id,
			_ => throw new NotSupportedException($"Update type {x.GetType().Name} is not supported.")
		});

		// Track which updates are bundled (should be removed from final list)
		HashSet<Guid> bundledUpdateIds = [];

		// Process each update to merge files from bundled updates
		for (int i = 0; i < updates.Count; i++)
		{
			if (updates[i] is Update update && update.BundledUpdates.Count > 0)
			{
				List<Models.File> mergedFiles = [.. update.Files];

				string? architecture = null;
				foreach (var currentBundledUpdate in update.BundledUpdates)
				{
					bundledUpdateIds.Add(currentBundledUpdate);

					if (!updateDictionary.TryGetValue(currentBundledUpdate, out var bundledIUpdate))
					{
						throw new InvalidOperationException($"Bundled update {currentBundledUpdate} for update {update.Id} not found in the list of updates.");
					}

					if (bundledIUpdate is not Update bundledUpdate)
					{
						throw new NotSupportedException($"Bundled update {currentBundledUpdate} for update {update.Id} is not a software update, which is not supported.");
					}

					// Pull the architecture from the bundled update
					architecture = bundledUpdate.Architecture;

					// Collect all files from bundled updates
					mergedFiles.AddRange(bundledUpdate.Files);
				}

				// Create a new Update with merged files
				updates[i] = update with { Architecture = architecture, Files = mergedFiles };
			}
		}

		// Remove bundled updates from the final list
		return [.. updates.Where(x => x switch
		{
			Update update => !bundledUpdateIds.Contains(update.Id),
			Driver driver => !bundledUpdateIds.Contains(driver.Id),
			_ => throw new NotSupportedException($"Update type {x.GetType().Name} is not supported.")
		})];
    }

	/// <summary>
	/// Creates an Update object from a SoftwareUpdate, resolving categories and localized properties.
	/// </summary>
	/// <param name="softwareUpdate">The SoftwareUpdate object to convert.</param>
	/// <param name="categories">The list of categories to resolve category names.</param>
	/// <param name="localizedProperties">The localized properties for the update.</param>
	/// <returns>An Update object with resolved properties.</returns>
	private static Update CreateSoftwareUpdate(SoftwareUpdate softwareUpdate, IReadOnlyList<Category> categories, LocalizedProperties localizedProperties) => new(
		Id: softwareUpdate.Id.Id,
		Title: localizedProperties.Title,
		Description: localizedProperties.Description,
		Architecture: (softwareUpdate
			.ApplicabilityRules?
			.Metadata?
			.PackageApplicabilityMetadata?
			.Assemblies is { Count: > 0 } assemblies ? assemblies[0] : null)?
			.AssemblyIdentity?
			.ProcessorArchitecture,
		CreationDate: softwareUpdate.UpdateProperties.CreationDate,
		KBArticleId: softwareUpdate.UpdateProperties.KBArticleId,
		Categories: [.. softwareUpdate
			.Relationships
			.Prerequisites
			.AtLeastOnePrerequisites
			.Where(x => x.IsCategory) // Only include categories, not other types of prerequisites
			.SelectMany(x => x.UpdateIdentities) // Flatten the list of update identities
			.Select(x => categories // Resolve the category name from the list of categories
				.FirstOrDefault(y => y.Id == x.Id)?.Name ?? 
				$"{x.Id}")], // Fallback to Id if the category is not found
		BundledUpdates: GetBundledUpdates(softwareUpdate),
		SupersededUpdates: [.. softwareUpdate.Relationships.SupersededUpdates?.Select(x => x.Id) ?? []],
		Files: [.. softwareUpdate
			.ResolvedFiles?
			.Select(x => new Models.File()
			{
				FileName = x.FileName,
				DownloadUri = x.Uri,
				ModifiedDate = x.Modified,
				Sha1Hash = x.Digest,
				Size = x.Size
			}) ?? []]
	);

	private static IReadOnlyList<Guid> GetBundledUpdates(SoftwareUpdate softwareUpdate)
	{
		var bundledUpdateIds = softwareUpdate.Relationships.BundledUpdates?.Select(x => x.Id) ?? [];
		var atLeastOneBundledUpdateIds = softwareUpdate
			.Relationships
			.BundledUpdates?
			.AtLeastOneBundled?
			.Where(x => !x.IsCategory)?
			.SelectMany(x => x.UpdateIdentities)?
			.Select(x => x.Id) ?? [];
		return [.. bundledUpdateIds.Concat(atLeastOneBundledUpdateIds)];
	}

	/// <summary>
	/// Creates a Driver object from a DriverUpdate, resolving categories and localized properties.
	/// </summary>
	/// <param name="driverUpdate">The DriverUpdate object to convert.</param>
	/// <param name="categories">The list of categories to resolve category names.</param>
	/// <param name="localizedProperties">The localized properties for the update.</param>
	/// <returns>A Driver object with resolved properties.</returns>
	private static Driver CreateDriverUpdate(DriverUpdate driverUpdate, IReadOnlyList<Category> categories, LocalizedProperties localizedProperties) => new(
		Id: driverUpdate.Id.Id,
		Title: localizedProperties.Title,
		Description: localizedProperties.Description,
		CreationDate: driverUpdate.UpdateProperties.CreationDate,
		Categories: [.. driverUpdate
			.Relationships
			.Prerequisites
			.AtLeastOnePrerequisites
			.Where(x => x.IsCategory)
			.SelectMany(x => x.UpdateIdentities)
			.Select(x => categories
				.FirstOrDefault(y => y.Id == x.Id)?.Name ?? 
				$"{x.Id}")] ,
		Files: [.. driverUpdate
			.ResolvedFiles?
			.Select(x => new Models.File()
			{
				FileName = x.FileName,
				DownloadUri = x.Uri,
				ModifiedDate = x.Modified,
				Sha1Hash = x.Digest,
				Size = x.Size
			}) ?? []]
	);

	/// <summary>
	/// Retrieves the localized properties for a given MicrosoftUpdatePackage and language. Provides a fallback to the default language if the specified language is not found.
	/// </summary>
	/// <param name="package">The MicrosoftUpdatePackage to retrieve localized properties from.</param>
	/// <param name="language">The language code for the desired localized properties. If null, the default language is used.</param>
	/// <returns>The localized properties for the specified language.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the localized properties for the specified language are not found.</exception>
	private static LocalizedProperties GetLocalizedProperties(MicrosoftUpdatePackage package, string? language = null)
	{
		// If no language is specified, use the default language for the package
		var defaultPackageLanguage = package.UpdateProperties.DefaultPropertiesLanguage;
		var packageLanguage = language ?? defaultPackageLanguage;

		// Try to find the localized properties for the specified language, or fall back to the default language if not found
		var localizedProperties = package.LocalizedProperties
			.FirstOrDefault(x => x.Language.Equals(packageLanguage, StringComparison.OrdinalIgnoreCase));
		localizedProperties ??= package.LocalizedProperties
				.FirstOrDefault(x => x.Language.Equals(defaultPackageLanguage, StringComparison.OrdinalIgnoreCase));

		// If still not found, throw an exception
		return localizedProperties ??
			throw new InvalidOperationException($"Localized properties not found for package {package.Id.Id} with language {packageLanguage}");
	}

	/// <summary>
	/// Retrieves driver updates from the Microsoft Update upstream source based on the provided computer IDs and PnP hardware IDs.
	/// </summary>
	/// <param name="progress"></param>
	/// <param name="computerIdsFilter"></param>
	/// <param name="pnpHardwareIdsFilter"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	//public static async IAsyncEnumerable<Update> GetDriversAsync(IProgress<DetailedProgress> progress,
	//	Guid[] computerIdsFilter,
	//	string[] pnpHardwareIdsFilter,
	//	[EnumeratorCancellation] CancellationToken cancellationToken = default)
	//{
	//       ArgumentNullException.ThrowIfNull(computerIdsFilter, nameof(computerIdsFilter));
	//       ArgumentNullException.ThrowIfNull(pnpHardwareIdsFilter, nameof(pnpHardwareIdsFilter));
	//       if (computerIdsFilter.Length == 0 || pnpHardwareIdsFilter.Length == 0)
	//	{
	//		throw new ArgumentException("At least one of Computer ID and one PnP Hardware ID must be provided.");
	//	}

	//	cancellationToken.ThrowIfCancellationRequested();

	//	UpstreamSource updatesSource = new(Endpoint.Default);
	//	var detailedProgress = new DetailedProgress("Retrieving updates", 0, 0);
	//	var internalProgress = new Progress<(int, int)>(x =>
	//	{
	//		var (Current, Total) = x;
	//		progress.Report(detailedProgress with { Count = Current, Total = Total });
	//	});

	//	// Filter so only the particular types and products wanted will be downloaded and processed
	//	var updatesFilter = new UpstreamDriverFilter()
	//	{
	//		ComputerFilter = computerIdsFilter,
	//		PnpHardwareFilter = pnpHardwareIdsFilter
	//	};

	//	// Iterate through all of the metadata returned by the upstream source
	//	await foreach (var current in updatesSource
	//		.GetDriversAsync(internalProgress, updatesFilter, cancellationToken: cancellationToken)
	//		.ConfigureAwait(false))
	//	{
	//		if (current is not DriverUpdate currentUpdate)
	//		{
	//			continue;
	//		}

	//		var localizedProperties = GetLocalizedProperties(currentUpdate);
	//		//yield return new Driver()
	//		//{
	//		//	Id = currentUpdate.Id.Id,
	//		//	Title = localizedProperties.Title,
	//		//	Description = localizedProperties.Description,
	//		//	CreationDate = currentUpdate.UpdateProperties.CreationDate,
	//		//	Categories = [],//[.. currentUpdate.Categories],
	//		//	SupersededUpdates = [.. currentUpdate.Relationships.SupersededUpdates?.Select(x => x.Id) ?? []],
	//		//	Files = []
	//		//	//[.. currentUpdate.Files
	//		//	//    .Select(y => new Models.File()
	//		//	//    {
	//		//	//        FileName = y.FileName,
	//		//	//        Source = y.Source,
	//		//	//        ModifiedDate = y.ModifiedDate,
	//		//	//        Digest = new FileDigest()
	//		//	//        {
	//		//	//            Algorithm = y.Digest.Algorithm,
	//		//	//            Value = y.Digest.HexString
	//		//	//        },
	//		//	//        Size = y.Size
	//		//	//    })]
	//		//};
	//	}
	//}
}
