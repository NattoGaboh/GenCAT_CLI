using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCAT_CLI.Commands
{
    public class AddEndpointSettings : CommandSettings
    {
        [CommandArgument(0, "<project>")]
        public string Project { get; set; } = string.Empty;

        [CommandArgument(0, "<module>")]
        public string Module { get; set; } = string.Empty;

        [CommandArgument(1, "<action>")]
        public string Action { get; set; } = string.Empty;
    }
}
