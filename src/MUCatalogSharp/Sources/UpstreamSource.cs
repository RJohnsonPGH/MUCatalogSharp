using MUCatalogSharp.Client;
using MUCatalogSharp.Metadata;
using System.Runtime.CompilerServices;

namespace MUCatalogSharp.Sources;

public class UpstreamSource(Endpoint upstreamEndpoint)
{
    private readonly UpstreamServerClient _Client = new(upstreamEndpoint);

	public async IAsyncEnumerable<MicrosoftUpdatePackage> GetCategoriesAsync(IProgress<(int, int)> progress, IEnumerable<Guid>? excludedPackageIds = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
		cancellationToken.ThrowIfCancellationRequested();

		excludedPackageIds ??= [];

		var identities = await _Client
			.GetCategoryIdsAsync()
			.ConfigureAwait(false);

		await foreach (var updatePackage in GetUpdateDataAsync(progress, identities, excludedPackageIds, cancellationToken))
		{
			yield return updatePackage;
		}
	}

	public async IAsyncEnumerable<MicrosoftUpdatePackage> GetUpdatesAsync(IProgress<(int, int)> progress, UpstreamSourceFilter filter, IEnumerable<Guid>? excludedPackageIds = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		excludedPackageIds ??= [];

		var identities = await _Client.GetUpdateIdsAsync(filter)
			.ConfigureAwait(false);

		await foreach (var updatePackage in GetUpdateDataAsync(progress, identities, excludedPackageIds, cancellationToken))
		{
			yield return updatePackage;
		}
	}

	public async IAsyncEnumerable<MicrosoftUpdatePackage> GetDriversAsync(IProgress<(int, int)> progress, UpstreamDriverFilter filter, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		
		var identities = await _Client.GetDriverIdsAsync(filter)
			.ConfigureAwait(false);

		await foreach (var driverPackage in GetUpdateDataAsync(progress, identities, [], cancellationToken))
		{
			yield return driverPackage;
		}
	}

	private async IAsyncEnumerable<MicrosoftUpdatePackage> GetUpdateDataAsync(
		IProgress<(int, int)> progress, 
		IEnumerable<MicrosoftUpdatePackageIdentity> identities, 
		IEnumerable<Guid> excludedPackageIds, 
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var includedPackages = identities
			.Where(identity => !excludedPackageIds.Any(excluded => identity.Id == excluded));

		int count = 0;
		int total = includedPackages.Count();
		var batches = includedPackages.Chunk(50);
		foreach (var batch in batches)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await foreach (var updatePackage in _Client
				.GetUpdateDataForIdsAsync(batch, cancellationToken)
				.ConfigureAwait(false))
			{
				progress.Report((++count, total));
				yield return updatePackage;
			}
		}

		yield break;
	}
}
