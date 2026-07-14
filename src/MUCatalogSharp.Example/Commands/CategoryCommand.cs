using MUCatalogSharp.Progress;
using Spectre.Console;
using Spectre.Console.Cli;

namespace MUCatalogSharp.Example.Commands;

public class CategoryCommand : AsyncCommand<CategoryCommand.Settings>
{
	public class Settings : CommandSettings { }

	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
	{
		var categories = new List<Models.Category>();

		await AnsiConsole.Progress()
			.Columns(
				new TaskDescriptionColumn(),
				new ProgressBarColumn(),
				new PercentageColumn(),
				new SpinnerColumn())
			.StartAsync(async ctx =>
			{
				var task = ctx.AddTask("[green]Retrieving categories[/]");
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

				await foreach (var category in UpdateRetriever.GetCategoriesAsync(progress))
				{
					categories.Add(category);
				}

				task.Value = task.MaxValue;
			});

		AnsiConsole.MarkupLine($"\n[bold green]Retrieved {categories.Count} categories:[/]\n");

		var table = new Table();
		table.AddColumn("Type");
		table.AddColumn("ID");
		table.AddColumn("Name");

		foreach (var category in categories.OrderBy(c => c.GetType().Name).ThenBy(c => c.Name))
		{
			var typeName = category.GetType().Name;
			var color = typeName switch
			{
				"Classification" => "blue",
				"Product" => "yellow",
				"Detectoid" => "cyan",
				_ => "white"
			};

			table.AddRow(
				$"[{color}]{typeName}[/]",
				category.Id.ToString(),
				category.Name);
		}

		AnsiConsole.Write(table);

		return 0;
	}
}
