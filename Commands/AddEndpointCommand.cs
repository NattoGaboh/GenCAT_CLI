using Spectre.Console.Cli;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCAT_CLI.Commands
{
    public class AddEndpointCommand : Command<AddEndpointSettings>
    {
        public override int Execute(CommandContext context, AddEndpointSettings settings, CancellationToken cancellationToken)
        {
            var module = settings.Module;
            var action = settings.Action;

            var generator = new EndpointGeneratorService();
            generator.Generate(module, action);

            AnsiConsole.MarkupLine($"[green]✅ Endpoint {module} {action} generado![/]");

            return 0;
        }
    }
}
