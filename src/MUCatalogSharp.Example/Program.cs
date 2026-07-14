using MUCatalogSharp.Example.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.AddCommand<CategoryCommand>("category")
        .WithDescription("Retrieve and display all Microsoft Update categories");

    config.AddCommand<UpdateCommand>("update")
        .WithDescription("Retrieve and display Microsoft Updates for a specific product and classification");
});

return await app.RunAsync(args);

