# MSUpdateAPI

A .NET library for querying and retrieving metadata from the Microsoft Windows Update service. This library provides a simple, modern API for accessing Windows Update categories, classifications, products, and update information programmatically.

## Description

MSUpdateAPI is built on top of Microsoft's update infrastructure and provides strongly-typed models for working with Windows Update data. The library handles the complexity of communicating with Microsoft's update services and exposes an easy-to-use API for retrieving update categories, metadata, files, and product hierarchies.

The library is designed for applications that need to query Windows Update information, such as update management tools, compliance systems, or automated patch deployment solutions.

## Features

- Asynchronous streaming API for efficient memory usage
- Strongly-typed models for updates, drivers, and categories
- Support for filtering by product and classification
- Progress reporting for long-running operations
- Built-in handling of bundled updates
- Support for .NET 10

## Installation

Add a reference to the `UpdateLib` project in your solution, or build and reference the compiled assembly.

## Basic Usage

### Retrieving Categories

Categories include classifications (e.g., Security Updates, Critical Updates) and products (e.g., Windows 10, Windows 11).

```csharp
using UpdateLib;
using UpdateLib.Progress;

var progress = new Progress<DetailedProgress>(p =>
{
    Console.WriteLine($"{p.Operation}: {p.Count}/{p.Total}");
});

await foreach (var category in UpdateRetriever.GetCategoriesAsync(progress))
{
    Console.WriteLine($"{category.Name} ({category.Id})");
}
```

### Filtering Categories by Type

You can filter categories to retrieve only specific types:

```csharp
using UpdateLib.Filters;

// Get only classifications
await foreach (var classification in UpdateRetriever.GetCategoriesAsync(
    progress, 
    CategoryTypeFilter.Classification))
{
    Console.WriteLine($"Classification: {classification.Name}");
}

// Get only products
await foreach (var product in UpdateRetriever.GetCategoriesAsync(
    progress, 
    CategoryTypeFilter.Product))
{
    Console.WriteLine($"Product: {product.Name}");
}
```

### Retrieving Updates for Specific Products and Classifications

To retrieve updates, you must specify at least one product GUID and one classification GUID. The library provides well-known constants for common products and classifications:

```csharp
using UpdateLib.Classifications;

var updates = await UpdateRetriever.GetUpdatesAsync(
    progress,
    productFilter: [WellKnownProduct.Windows10],
    classificationFilter: [WellKnownClassification.SecurityUpdates]);

foreach (var update in updates)
{
    if (update is UpdateLib.Models.Update softwareUpdate)
    {
        Console.WriteLine($"Update: {softwareUpdate.Title}");
        Console.WriteLine($"  KB Article: {softwareUpdate.KBArticleId}");
        Console.WriteLine($"  Severity: {softwareUpdate.Severity}");
        Console.WriteLine($"  Support URL: {softwareUpdate.SupportUrl}");
    }
    else if (update is UpdateLib.Models.Driver driver)
    {
        Console.WriteLine($"Driver: {driver.Title}");
        Console.WriteLine($"  Provider: {driver.DriverProvider}");
        Console.WriteLine($"  Version: {driver.DriverVerDate}");
    }
}
```

Available well-known products include:
- `WellKnownProduct.Windows10`
- `WellKnownProduct.Windows11`
- `WellKnownProduct.WindowsServer2019`
- `WellKnownProduct.WindowsServer2022`
- `WellKnownProduct.WindowsServer2025`
- `WellKnownProduct.Windows10Drivers` (for driver updates)
- `WellKnownProduct.Windows11Drivers` (for driver updates)

Available well-known classifications include:
- `WellKnownClassification.SecurityUpdates`
- `WellKnownClassification.CriticalUpdates`
- `WellKnownClassification.Updates`
- `WellKnownClassification.UpdateRollups`
- `WellKnownClassification.ServicePacks`
- `WellKnownClassification.FeaturePacks`
- `WellKnownClassification.Drivers`
- `WellKnownClassification.Tools`
- `WellKnownClassification.Upgrades`
- `WellKnownClassification.DefinitionUpdates`

**Important:** Drivers have separate product identifiers. To retrieve driver updates, you must use the driver-specific product (e.g., `WellKnownProduct.Windows11Drivers`) along with the `WellKnownClassification.Drivers` classification:

```csharp
using UpdateLib.Classifications;

// Correct way to retrieve driver updates for Windows 11
var driverUpdates = await UpdateRetriever.GetUpdatesAsync(
    progress,
    productFilter: [WellKnownProduct.Windows11Drivers],
    classificationFilter: [WellKnownClassification.Drivers]);

foreach (var update in driverUpdates)
{
    if (update is UpdateLib.Models.Driver driver)
    {
        Console.WriteLine($"Driver: {driver.Title}");
        Console.WriteLine($"  Provider: {driver.DriverProvider}");
        Console.WriteLine($"  Version: {driver.DriverVerDate}");
        Console.WriteLine($"  Class: {driver.DriverClass}");
    }
}
```

**Note:** Retrieving driver updates can be extremely time-consuming. During testing, retrieving all drivers for Windows 11 took over 90 minutes due to the large number of available drivers.

### Accessing Update Files

Each update contains information about its associated files:

```csharp
using UpdateLib.Classifications;

var updates = await UpdateRetriever.GetUpdatesAsync(
    progress,
    productFilter: [WellKnownProduct.Windows11],
    classificationFilter: [WellKnownClassification.CriticalUpdates]);

foreach (var update in updates)
{
    if (update is UpdateLib.Models.Update softwareUpdate)
    {
        Console.WriteLine($"\nUpdate: {softwareUpdate.Title}");
        Console.WriteLine("Files:");

        foreach (var file in softwareUpdate.Files)
        {
            Console.WriteLine($"  - {file.FileName}");
            Console.WriteLine($"    URL: {file.DownloadUrl}");
            Console.WriteLine($"    Size: {file.Size:N0} bytes");
            Console.WriteLine($"    SHA256: {file.Digest.DigestBase64}");
        }
    }
}
}
```

### Cancellation Support

All asynchronous operations support cancellation tokens:

```csharp
using var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(30));

try
{
    await foreach (var category in UpdateRetriever.GetCategoriesAsync(
        progress, 
        cancellationToken: cts.Token))
    {
        Console.WriteLine(category.Name);
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled");
}
```

## Example Application

The solution includes a complete example application (`UpdateLib.Example`) that demonstrates how to use the library with a command-line interface. The example uses Spectre.Console for rich terminal output.

To run the example:

```bash
# List all categories
dotnet run --project UpdateLib.Example category

# Get updates for a specific product and classification
dotnet run --project UpdateLib.Example update <PRODUCT_GUID> <CLASSIFICATION_GUID>
```

## Models

The library provides the following main model types:

- `Category`: Base class for all category types
- `Classification`: Security Updates, Critical Updates, etc.
- `Product`: Windows products and versions
- `Detectoid`: Detection rules for update applicability
- `IUpdate`: Interface for all update types
- `Update`: Software updates
- `Driver`: Driver updates
- `File`: Update file information with download URLs and checksums

## Requirements

- .NET 10 or later
- Network access to Microsoft Update services

## Acknowledgments

The original implementation of the Windows Update protocol handling was adapted from Microsoft's [update-server-server-sync](https://github.com/microsoft/update-server-server-sync) repository. The code has been heavily modified and rewritten to add asynchronous functionality, reduce complexity, and provide a more developer-friendly API surface.

## License

[MIT](https://choosealicense.com/licenses/mit/)