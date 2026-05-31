using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCAT_CLI.Services
{
    public class EndpointGeneratorService
    {
        public void Generate(string project, string module, string action)
        {
            var root = Path.Combine(Directory.GetCurrentDirectory(), "..", "projects");
            //var root = Directory.GetCurrentDirectory();

            //var solutionName = Directory.GetDirectories(root)
            //    .Select(Path.GetFileName)
            //    .FirstOrDefault();

            var solutionName = project;

            var src = Path.Combine(root, solutionName!, "src");

            var domainPath = Path.Combine(src, $"{solutionName}.Domain", "Entities");
            var appPath = Path.Combine(src, $"{solutionName}.Application", "Features", module + "s");
            var apiPath = Path.Combine(src, $"{solutionName}.Api", "Endpoints");

            Directory.CreateDirectory(domainPath);
            Directory.CreateDirectory(appPath);
            Directory.CreateDirectory(apiPath);

            RegisterEndpoint(apiPath,module);
            InjectServicesInProgram(apiPath);

            GenerateEntity(solutionName,domainPath, module);
            GenerateDto(solutionName,appPath, module);
            GenerateQuery(solutionName, appPath, module, action);
            GenerateHandler(solutionName, appPath, module, action);
            GenerateEndpoint(solutionName, apiPath, module);
            GenerateEndpointExtensions(solutionName, apiPath);
        }


        private static void RegisterEndpoint(string apiPath, string module)
        {
            var programFile = Path.Combine(apiPath,"..", "Program.cs");

            var content = File.ReadAllText(programFile);

            var methodCall = $"app.MapEndpoints();";

            if (content.Contains(methodCall))
                return;

            // Insertar antes de app.Run();
            content = content.Replace(
                "app.Run();",
                $@"
app.MapEndpoints();

app.Run();"
            );

            File.WriteAllText(programFile, content);
        }

        private static void InjectServicesInProgram(string apiPath)
        {
            var programFile = Path.Combine(apiPath,"..", "Program.cs");

            var content = File.ReadAllText(programFile);

            if (!content.Contains("AddApplication"))
            {
                content = content.Replace(
                    "var app = builder.Build();",
                    @"
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();"
                );
            }

            File.WriteAllText(programFile, content);
        }

        private static void GenerateEntity(string project, string path, string module)
        {
            var file = Path.Combine(path, $"{module}.cs");

            if (File.Exists(file)) return;

            File.WriteAllText(file, $@"
namespace {project}.Domain.Entities;

public class {module}
{{
    public Guid Id {{ get; set; }}
    public string Nombre {{ get; set; }} = string.Empty;
}}
");
        }

        private static void GenerateDto(string project,string path, string module)
        {
            var dtoPath = Path.Combine(path, "DTOs");
            Directory.CreateDirectory(dtoPath);

            var file = Path.Combine(dtoPath, $"{module}Dto.cs");

            File.WriteAllText(file, $@"
namespace {project}.Application.Features.{module}s.DTOs;

public class {module}Dto
{{
    public Guid Id {{ get; set; }}
    public string Nombre {{ get; set; }} = string.Empty;
}}
");
        }

        private static void GenerateQuery(string project,string path, string module, string action)
        {
            var queryPath = Path.Combine(path, "Queries", $"Get{module}s");
            Directory.CreateDirectory(queryPath);

            var file = Path.Combine(queryPath, $"Get{module}sQuery.cs");

            File.WriteAllText(file, $@"
using MediatR;
using {project}.Application.Features.{module}s.DTOs;

namespace {project}.Application.Features.{module}s.Queries;

public record Get{module}sQuery() : IRequest<List<{module}Dto>>;
");
        }

        private static void GenerateHandler(string project, string path, string module, string action)
        {
            var handlerPath = Path.Combine(path, "Queries", $"Get{module}s");
            var file = Path.Combine(handlerPath, $"Get{module}sHandler.cs");

            File.WriteAllText(file, $@"
using Dapper;
using MediatR;
using {project}.Application.Interfaces;
using {project}.Application.Features.{module}s.DTOs;

namespace {project}.Application.Features.{module}s.Queries;

public class Get{module}sHandler : IRequestHandler<Get{module}sQuery, List<{module}Dto>>
{{
    private readonly IDbConnectionFactory _factory;

    public Get{module}sHandler(IDbConnectionFactory factory)
    {{
        _factory = factory;
    }}

    public async Task<List<{module}Dto>> Handle(Get{module}sQuery request, CancellationToken cancellationToken)
    {{
        using var connection = _factory.CreateConnection();

        var sql = ""SELECT id, nombre FROM {module}s"";

        var result = await connection.QueryAsync<{module}Dto>(sql);

        return result.ToList();
    }}
}}
");
        }

        private static void GenerateEndpoint(string project, string path, string module)
        {
            var file = Path.Combine(path, $"{module}sEndpoints.cs");

            var content = $@"
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace {project}.Api.Endpoints;

public static class {module}sEndpoints
{{
    public static void Map(IEndpointRouteBuilder app)
    {{
        app.MapGet(""/api/{module.ToLower()}s"", async (IMediator mediator) =>
        {{
            return await mediator.Send(new Get{module}sQuery());
        }});
    }}
}}
";

            File.WriteAllText(file, content);
        }

        private static void GenerateEndpointExtensions(string project, string path)
        {
            var queryPath = Path.Combine(path,"..","Extensions");
            Directory.CreateDirectory(queryPath);

            var file = Path.Combine(queryPath, "EndpointExtensions.cs");

            var content = $@"
using System.Reflection;

namespace {project}.Api.Extensions;

public static class EndpointExtensions
{{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {{
        var assembly = Assembly.GetExecutingAssembly();

        var endpointTypes = assembly.GetTypes()
            .Where(t => t.IsClass &&
                        t.IsAbstract &&
                        t.IsSealed &&
                        t.Name.EndsWith(""Endpoints""));

        foreach (var type in endpointTypes)
        {{
            var method = type.GetMethod(""Map"", BindingFlags.Public | BindingFlags.Static);

            if (method != null)
            {{
                method.Invoke(null, new object[] {{ app }});
            }}
        }}
    }}
}}
";
            File.WriteAllText(file, content);
        }
    }
}
