using Spectre.Console.Cli;
using GenCAT_CLI.Commands;

var app = new CommandApp();

app.Configure(config =>
{
    config.AddCommand<NewProjectCommand>("new")
          .WithDescription("Crea un nuevo proyecto con Clean Architecture");
    config.AddCommand<AddEndpointCommand>("add-endpoint");
});


return app.Run(args);