using MUCatalogSharp.Classifications;
using MUCatalogSharp.Models;
using MUCatalogSharp.Progress;

namespace MUCatalogSharp.Tests.Integration;

public class UpdateRetrieverTests
{
	[Fact]
	public async Task GetCategoriesAsync_ShouldReturnCategories()
	{
		// Arrange & Act
		var categories = new List<Category>();
		await foreach (var category in UpdateRetriever
			.GetCategoriesAsync(new Progress<DetailedProgress>(), cancellationToken: TestContext.Current.CancellationToken)
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
		Guid[] productFilter = [WellKnownProduct.Windows11];
		Guid[] classificationFilter = [WellKnownClassification.SecurityUpdates];

		// Act
		var updates = new List<IUpdate>();
		foreach (var update in await UpdateRetriever
			.GetUpdatesAsync(new Progress<DetailedProgress>(), productFilter, classificationFilter, TestContext.Current.CancellationToken))
		{
			updates.Add(update);
		}

		// Assert
		Assert.NotEmpty(updates);
	}

	[Fact]
	public async Task GetDriverUpdatesAsync_ShouldReturnDrivers()
	{
		// Arrange
		Guid[] productFilter = [new("bfeb1830-4b34-40d0-a2d7-8d6f994ddb58")]; // W11 22H2 and later - Drivers
		Guid[] classificationFilter = [WellKnownClassification.Drivers];

		// Act
		var updates = new List<IUpdate>();
		foreach (var update in await UpdateRetriever
			.GetUpdatesAsync(new Progress<DetailedProgress>(), productFilter, classificationFilter, TestContext.Current.CancellationToken))
		{
			updates.Add(update);
		}

		// Assert
		Assert.NotEmpty(updates);
	}

	//[Fact]
	//public async Task GetDriversAsync_ShouldReturnDrivers()
	//{
	//	// Arrange
	//	Guid[] hardwareIds = [
	//		new("3962b55f-f768-58cb-9ca2-b5d61a1fdbae")
	//	];
	//	string[] pnpHardwareIds = [
	//		@"USB\VID_8087&PID_0A2B&REV_0010",
	//	];

	//	// Act
	//	await foreach (var driver in UpdateRetriever
	//		.GetDriversAsync(new Progress<DetailedProgress>(), hardwareIds, pnpHardwareIds, TestContext.Current.CancellationToken)
	//		.ConfigureAwait(false))
	//	{
	//		Assert.NotNull(driver);
	//	}
	//}
}
