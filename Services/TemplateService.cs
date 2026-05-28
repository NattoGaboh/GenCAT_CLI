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
            var root = Path.Combine(Directory.GetCurrentDirectory(), "..", "projects", options.Name);
            var src = Path.Combine(root, "src");

            Directory.CreateDirectory(root);
            Directory.CreateDirectory(src);

            // 1. Crear solución
            _cli.Run($"new sln -n {options.Name}", root);

            // 2. Crear proyectos
            CreateProjects(options, src);

            // 3. Agregar proyectos al .sln
            AddProjectsToSolution(options, root);

            // 4. Agregar referencias
            AddReferences(options, src);

            // 5. Generar contenido base (templates)
            GenerateBaseFiles(options, src);
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
            _ = Path.Combine(root, "src");

            _cli.Run($"sln add src/{options.Name}.Api/{options.Name}.Api.csproj", root);
            _cli.Run($"sln add src/{options.Name}.Application/{options.Name}.Application.csproj", root);
            _cli.Run($"sln add src/{options.Name}.Domain/{options.Name}.Domain.csproj", root);
            _cli.Run($"sln add src/{options.Name}.Infrastructure/{options.Name}.Infrastructure.csproj", root);
        }


        private void AddReferences(ProjectOptions options, string src)
        {
            var api = $"{options.Name}.Api";
            var app = $"{options.Name}.Application";
            var domain = $"{options.Name}.Domain";
            var infra = $"{options.Name}.Infrastructure";

            _cli.Run($"add {api}/{api}.csproj reference {app}/{app}.csproj", src);
            _cli.Run($"add {api}/{api}.csproj reference {infra}/{infra}.csproj", src);

            _cli.Run($"add {app}/{app}.csproj reference {domain}/{domain}.csproj", src);

            _cli.Run($"add {infra}/{infra}.csproj reference {app}/{app}.csproj", src);
        }


        private static void GenerateBaseFiles(ProjectOptions options, string src)
        {
            var domainPath = Path.Combine(src, $"{options.Name}.Domain", "BaseEntity.cs");

            File.WriteAllText(domainPath, $@"
namespace {options.Name}.Domain;

public abstract class BaseEntity
{{
    public Guid Id {{ get; set; }}
}}
");

            var appPath = Path.Combine(src, $"{options.Name}.Application", "DependencyInjection.cs");

            File.WriteAllText(appPath, $@"
namespace {options.Name}.Application;

public static class DependencyInjection
{{
    public static void AddApplication() {{ }}
}}
");

            var infraPath = Path.Combine(src, $"{options.Name}.Infrastructure", "DependencyInjection.cs");

            File.WriteAllText(infraPath, $@"
namespace {options.Name}.Infrastructure;

public static class DependencyInjection
{{
    public static void AddInfrastructure() {{ }}
}}
");
        }
    }
}
