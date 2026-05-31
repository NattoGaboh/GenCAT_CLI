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

            // 6. Instalar Paquetes
            InstallPackages(options, src);

            // 7. Generate AppSettigns
            GenerateAppSettings(options, src);

            // 8. Install MediatR
            InstallMediatR(options, src);

            // 9. RegisterDependencyInjection
            RegisterDependencyInjectionApp(options, src);

            // 10.
            GenerateIDbConnectionFactory(options,src);

            // 11.
            GenerateConnectionFactory(options, src);

            // 12.
            RegisterDependencyInjectionInfra(options, src);
        }

        private void CreateProjects(ProjectOptions options, string src)
        {
            //_cli.Run($"new webapi --use-controllers -n {options.Name}.Api", src);
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

        private void InstallPackages(ProjectOptions options, string src)
        {
            var infra = $"{options.Name}.Infrastructure";

            var cli = new DotnetCliService();

            // Dapper
            cli.Run($"add {infra}/{infra}.csproj package Dapper", src);

            // DB providers
            if (options.Database == "PostgreSQL")
                cli.Run($"add {infra}/{infra}.csproj package Npgsql", src);

            if (options.Database == "SQL Server")
                cli.Run($"add {infra}/{infra}.csproj package Microsoft.Data.SqlClient", src);

            // Configuración
            cli.Run($"add {infra}/{infra}.csproj package Microsoft.Extensions.Configuration", src);
        }

        private void GenerateAppSettings(ProjectOptions options, string src)
        {
            var apiPath = Path.Combine(src, $"{options.Name}.Api");

            var generator = new AppSettingsGenerator();
            generator.Generate(options, apiPath);
        }

        private void InstallMediatR(ProjectOptions options, string src)
        {
            var app = $"{options.Name}.Application";
            var api = $"{options.Name}.Api";

            var cli = new DotnetCliService();

            cli.Run($"add {api}/{api}.csproj package MediatR", src);
            cli.Run($"add {app}/{app}.csproj package MediatR", src);
            cli.Run($"add {app}/{app}.csproj package MediatR.Extensions.Microsoft.DependencyInjection", src);
        }

        private static void RegisterDependencyInjectionApp(ProjectOptions options, string src)
        {
            var file = Path.Combine(src, $"{options.Name}.Application", "DependencyInjection.cs");

            if (File.Exists(file)) File.Delete(file);

            File.WriteAllText(file, $@"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace {options.Name}.Application;

public static class DependencyInjection
{{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {{
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }}
}}");
        }

        private static void RegisterDependencyInjectionInfra(ProjectOptions options, string src)
        {
            var file = Path.Combine(src, $"{options.Name}.Infrastructure", "DependencyInjection.cs");

            if (File.Exists(file)) File.Delete(file);

            File.WriteAllText(file, $@"
using Microsoft.Extensions.DependencyInjection;
using {options.Name}.Infrastructure.Persistence;
using {options.Name}.Application.Interfaces;

namespace {options.Name}.Infrastructure;

public static class DependencyInjection
{{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {{
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        return services;
    }}
}}");
        }

        private static void GenerateIDbConnectionFactory(ProjectOptions options, string src)
        {
            var queryPath = Path.Combine(src,$"{options.Name}.Application","Interfaces");
            Directory.CreateDirectory(queryPath);

            var file = Path.Combine(queryPath, "IDbConnectionFactory.cs");
            if (File.Exists(file)) return;

            File.WriteAllText(file, $@"
using System.Data;

namespace {options.Name}.Application.Interfaces;

public interface IDbConnectionFactory
{{
    IDbConnection CreateConnection();
}}");
        }

        private static void GenerateConnectionFactory(ProjectOptions options, string src)
        {
            var queryPath = Path.Combine(src, $"{options.Name}.Infrastructure", "Persistence");
            Directory.CreateDirectory(queryPath);

            var file = Path.Combine(queryPath, "DbConnectionFactory.cs");
            if (File.Exists(file)) return;

            File.WriteAllText(file, $@"
using System.Data;
using Microsoft.Extensions.Configuration;
using {options.Name}.Application.Interfaces;

namespace {options.Name}.Infrastructure.Persistence;

public class DbConnectionFactory : IDbConnectionFactory
{{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {{
        _configuration = configuration;
    }}

    public IDbConnection CreateConnection()
    {{
        var connectionString = _configuration.GetConnectionString(""DefaultConnection"");

        // PostgreSQL
        if (_configuration[""Database""] == ""PostgreSQL"")
            return new Npgsql.NpgsqlConnection(connectionString);

        // SQL Server
        return new Microsoft.Data.SqlClient.SqlConnection(connectionString);
    }}
}}");
        }
    }
}
