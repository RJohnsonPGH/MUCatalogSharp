using MUCatalogSharp.Classifications;
using MUCatalogSharp.Client;
using MUCatalogSharp.Metadata;
using MUCatalogSharp.Sources;

namespace MUCatalogSharp.Tests.Integration;

public partial class UpstreamSourceTests
{
	[Fact]
	public async Task GetCategoriesAsync_ShouldReturnCategories()
	{
		// Arrange
		var source = new UpstreamSource(Endpoint.Default);

		// Act
		var categories = new List<MicrosoftUpdatePackage>();
		await foreach (var category in source
			.GetCategoriesAsync(new Progress<(int, int)>(), cancellationToken: TestContext.Current.CancellationToken)
			.ConfigureAwait(false))
		{
			categories.Add(category);
		}

		// Assert
		Assert.NotEmpty(categories);
	}

	[Fact]
	public async Task GetUpdatesAsync_ShouldReturnUpdates()
	{
		// Arrange
		var source = new UpstreamSource(Endpoint.Default);
		var filter = new UpstreamSourceFilter()
		{
			ProductsFilter = [WellKnownProduct.Windows11],
			ClassificationsFilter = [WellKnownClassification.SecurityUpdates]
		};

		// Act
		var updates = new List<MicrosoftUpdatePackage>();
		await foreach (var update in source
			.GetUpdatesAsync(new Progress<(int, int)>(), filter, cancellationToken: TestContext.Current.CancellationToken)
			.ConfigureAwait(false))
		{
			updates.Add(update);
		}

		// Assert
		Assert.NotEmpty(updates);
	}
}
