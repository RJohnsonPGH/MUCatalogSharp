using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using MUCatalogSharp.Progress;
using MUCatalogSharp;
using MUCatalogSharp.Models;

namespace MUCatalogSharp.Example.Commands;

public class UpdateCommand : AsyncCommand<UpdateCommand.Settings>
{
	public class Settings : CommandSettings
	{
		[CommandArgument(0, "<PRODUCT_GUID>")]
		[Description("The GUID of the product to filter updates by")]
		public Guid ProductGuid { get; set; }

		[CommandArgument(1, "<CLASSIFICATION_GUID>")]
		[Description("The GUID of the classification to filter updates by")]
		public Guid ClassificationGuid { get; set; }
	}

	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
	{
		var updates = new List<IUpdate>();

		await AnsiConsole.Progress()
			.Columns(
				new TaskDescriptionColumn(),
				new ProgressBarColumn(),
				new PercentageColumn(),
				new SpinnerColumn())
			.StartAsync(async ctx =>
			{
				var task = ctx.AddTask("[green]Retrieving updates[/]");
				task.IsIndeterminate = true;

				var progress = new Progress<DetailedProgress>(p =>
				{
					if (p.Total > 0)
					{
						task.IsIndeterminate = false;
						task.MaxValue = p.Total;
						task.Value = p.Count;
						task.Description = $"[green]{p.Operation}[/]";
					}
				});

				foreach (var update in await UpdateRetriever.GetUpdatesAsync(
					progress,
					productFilter: [settings.ProductGuid],
					classificationFilter: [settings.ClassificationGuid])
					.ConfigureAwait(false))
				{
					updates.Add(update);
				}

				task.Value = task.MaxValue;
			});

		AnsiConsole.MarkupLine($"\n[bold green]Retrieved {updates.Count} updates[/]\n");

		if (updates.Count == 0)
		{
			AnsiConsole.MarkupLine("[yellow]No updates found for the specified product and classification.[/]");
			return 0;
		}

		// Interactive selection loop
		while (true)
		{
			AnsiConsole.Clear();
			AnsiConsole.MarkupLine($"[bold cyan]Select an update to view details[/] ([dim]Total: {updates.Count}[/])\n");

			var orderedUpdates = updates.OrderBy(u => u switch
			{
				Models.Update software => software.Title,
				Models.Driver driver => driver.Title,
				_ => string.Empty
			}).ToList();

			var choices = orderedUpdates.Select(u => u switch
			{
				Models.Update software => $"[cyan]Software[/] - {software.Title.EscapeMarkup()}",
				Models.Driver driver => $"[yellow]Driver[/] - {driver.Title.EscapeMarkup()}",
				_ => "Unknown"
			}).ToList();

			choices.Add("[red]Exit[/]");

			var selection = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
					.Title("[green]Choose an update:[/]")
					.PageSize(10)
					.MoreChoicesText("[grey](Move up and down to reveal more updates)[/]")
					.AddChoices(choices));

			if (selection == "[red]Exit[/]")
			{
				break;
			}

			var selectedIndex = choices.IndexOf(selection);
			var selectedUpdate = orderedUpdates[selectedIndex];

			ShowDetailedView(selectedUpdate);

			AnsiConsole.WriteLine();
			AnsiConsole.Markup("[dim]Press any key to return to the list...[/]");
			Console.ReadKey(true);
		}

		return 0;
	}

	private static void ShowDetailedView(IUpdate update)
	{
		AnsiConsole.Clear();

		switch (update)
		{
			case Models.Update software:
				ShowSoftwareUpdateDetails(software);
				break;
			case Models.Driver driver:
				ShowDriverUpdateDetails(driver);
				break;
		}
	}

	private static void ShowSoftwareUpdateDetails(Models.Update update)
	{
		var panel = new Panel(new Markup($"[bold yellow]{update.Title.EscapeMarkup()}[/]"))
		{
			Header = new PanelHeader("[cyan]Software Update Details[/]"),
			Border = BoxBorder.Rounded
		};
		AnsiConsole.Write(panel);
		AnsiConsole.WriteLine();

		var table = new Table();
		table.Border(TableBorder.Rounded);
		table.AddColumn("[bold]Property[/]");
		table.AddColumn("[bold]Value[/]");

		table.AddRow("Update ID", update.Id.ToString());
		table.AddRow("Title", update.Title.EscapeMarkup());
		table.AddRow("Description", string.IsNullOrWhiteSpace(update.Description) ? "[dim]N/A[/]" : update.Description.EscapeMarkup());
		table.AddRow("Creation Date", update.CreationDate.ToString("yyyy-MM-dd HH:mm:ss"));
		table.AddRow("Architecture", string.IsNullOrWhiteSpace(update.Architecture) ? "[dim]N/A[/]" : update.Architecture);
		table.AddRow("KB Article ID", string.IsNullOrWhiteSpace(update.KBArticleId) ? "[dim]N/A[/]" : update.KBArticleId);
		table.AddRow("Categories", update.Categories.Count > 0 ? string.Join("\n", update.Categories) : "[dim]None[/]");
		table.AddRow("Bundled Updates", update.BundledUpdates.Count > 0 ? string.Join("\n", update.BundledUpdates) : "[dim]None[/]");
		table.AddRow("Superseded Updates", update.SupersededUpdates.Count > 0 ? string.Join("\n", update.SupersededUpdates) : "[dim]None[/]");
		table.AddRow("Files", update.Files.Count.ToString());

		AnsiConsole.Write(table);

		if (update.Files.Count > 0)
		{
			AnsiConsole.WriteLine();
			AnsiConsole.MarkupLine("[bold cyan]Files:[/]");
			var filesTable = new Table();
			filesTable.Border(TableBorder.Rounded);
			filesTable.AddColumn("File Name");
			filesTable.AddColumn("Size");
			filesTable.AddColumn("Modified Date");
			filesTable.AddColumn("SHA1 Hash");

			foreach (var file in update.Files)
			{
				filesTable.AddRow(
					file.FileName.EscapeMarkup(),
					FormatFileSize(file.Size),
					file.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"),
					file.Sha1Hash);
			}

			AnsiConsole.Write(filesTable);
		}
	}

	private static void ShowDriverUpdateDetails(Models.Driver driver)
	{
		var panel = new Panel(new Markup($"[bold yellow]{driver.Title.EscapeMarkup()}[/]"))
		{
			Header = new PanelHeader("[cyan]Driver Update Details[/]"),
			Border = BoxBorder.Rounded
		};
		AnsiConsole.Write(panel);
		AnsiConsole.WriteLine();

		var table = new Table();
		table.Border(TableBorder.Rounded);
		table.AddColumn("[bold]Property[/]");
		table.AddColumn("[bold]Value[/]");

		table.AddRow("Update ID", driver.Id.ToString());
		table.AddRow("Title", driver.Title.EscapeMarkup());
		table.AddRow("Description", string.IsNullOrWhiteSpace(driver.Description) ? "[dim]N/A[/]" : driver.Description.EscapeMarkup());
		table.AddRow("Creation Date", driver.CreationDate.ToString("yyyy-MM-dd HH:mm:ss"));
		table.AddRow("Categories", driver.Categories.Count > 0 ? string.Join("\n", driver.Categories) : "[dim]None[/]");
		table.AddRow("Files", driver.Files.Count.ToString());

		AnsiConsole.Write(table);

		if (driver.Files.Count > 0)
		{
			AnsiConsole.WriteLine();
			AnsiConsole.MarkupLine("[bold cyan]Files:[/]");
			var filesTable = new Table();
			filesTable.Border(TableBorder.Rounded);
			filesTable.AddColumn("File Name");
			filesTable.AddColumn("Size");
			filesTable.AddColumn("Modified Date");
			filesTable.AddColumn("SHA1 Hash");

			foreach (var file in driver.Files)
			{
				filesTable.AddRow(
					file.FileName.EscapeMarkup(),
					FormatFileSize(file.Size),
					file.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"),
					file.Sha1Hash);
			}

			AnsiConsole.Write(filesTable);
		}
	}

	private static string FormatFileSize(ulong bytes)
	{
		string[] sizes = ["B", "KB", "MB", "GB", "TB"];
		double len = bytes;
		int order = 0;
		while (len >= 1024 && order < sizes.Length - 1)
		{
			order++;
			len /= 1024;
		}
		return $"{len:0.##} {sizes[order]}";
	}
}
