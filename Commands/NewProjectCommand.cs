using Spectre.Console.Cli;
using Spectre.Console;
using GenCAT_CLI.Models;
using GenCAT_CLI.Services;

namespace GenCAT_CLI.Commands;

    public class NewProjectCommand : Command
    {
        public override int Execute(CommandContext context, CancellationToken cancellation)
        {
            AnsiConsole.MarkupLine("[green] Generador de proyectos Clean Architecture[/]");

            var projectName = AnsiConsole.Ask<string>("Nombre del proyecto:");

            var db = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecciona base de datos:")
                    .AddChoices("PostgreSQL", "SQL Server")
            );

            var useJwt = AnsiConsole.Confirm("¿Incluir autenticación JWT?");
            var multiTenant = AnsiConsole.Confirm("¿Soporte Multi-tenant?");

            var options = new ProjectOptions
            {
                Name = projectName,
                Database = db,
                UseJwt = useJwt,
                MultiTenant = multiTenant
            };

            var templateService = new TemplateService();
            templateService.GenerateProject(options);

            AnsiConsole.MarkupLine($"[green] Proyecto {projectName} creado exitosamente![/]");

            return 0;
        }
    }

