using GenCAT_CLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCAT_CLI.Services
{
    public class TemplateService
    {
        private readonly DotnetCliService _cli = new();

        public void GenerateProject(ProjectOptions options)
        {
            var root = Path.Combine(Directory.GetCurrentDirectory(), options.Name);
            var src = Path.Combine(root, "src");

            Directory.CreateDirectory(root);
            Directory.CreateDirectory(src);

            // 1. Crear solución
            _cli.Run($"new sln -n {options.Name}", root);

            // 2. Crear proyectos
            CreateProjects(options, src);

            // 3. Agregar proyectos al .sln
            AddProjectsToSolution(options, root);
        }

        private void CreateProjects(ProjectOptions options, string src)
        {
            _cli.Run($"new webapi -n {options.Name}.Api", src);
            _cli.Run($"new classlib -n {options.Name}.Application", src);
            _cli.Run($"new classlib -n {options.Name}.Domain", src);
            _cli.Run($"new classlib -n {options.Name}.Infrastructure", src);
        }

        private void AddProjectsToSolution(ProjectOptions options, string root)
        {
            var src = Path.Combine(root, "src");

            _cli.Run($"sln add src/{options.Name}.Api/{options.Name}.Api.csproj", root);
            _cli.Run($"sln add src/{options.Name}.Application/{options.Name}.Application.csproj", root);
            _cli.Run($"sln add src/{options.Name}.Domain/{options.Name}.Domain.csproj", root);
            _cli.Run($"sln add src/{options.Name}.Infrastructure/{options.Name}.Infrastructure.csproj", root);
        }
    }
}
