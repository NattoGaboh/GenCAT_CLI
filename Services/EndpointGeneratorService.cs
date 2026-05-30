using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCAT_CLI.Services
{
    public class EndpointGeneratorService
    {
        public void Generate(string module, string action)
        {
            var root = Directory.GetCurrentDirectory();

            var solutionName = Directory.GetDirectories(root)
                .Select(Path.GetFileName)
                .FirstOrDefault();

            var src = Path.Combine(root, solutionName!, "src");

            var domainPath = Path.Combine(src, $"{solutionName}.Domain", "Entities");
            var appPath = Path.Combine(src, $"{solutionName}.Application", "Features", module + "s");
            var apiPath = Path.Combine(src, $"{solutionName}.Api", "Endpoints");

            Directory.CreateDirectory(domainPath);
            Directory.CreateDirectory(appPath);
            Directory.CreateDirectory(apiPath);

            GenerateEntity(domainPath, module);
            GenerateDto(appPath, module);
            GenerateQuery(appPath, module, action);
            GenerateHandler(appPath, module, action);
            GenerateEndpoint(apiPath, module, action);
        }

        private void GenerateEntity(string path, string module)
        {
            var file = Path.Combine(path, $"{module}.cs");

            if (File.Exists(file)) return;

            File.WriteAllText(file, $@"
namespace Domain.Entities;

public class {module}
{{
    public Guid Id {{ get; set; }}
    public string Nombre {{ get; set; }} = string.Empty;
}}
");
        }

        private void GenerateDto(string path, string module)
        {
            var dtoPath = Path.Combine(path, "DTOs");
            Directory.CreateDirectory(dtoPath);

            var file = Path.Combine(dtoPath, $"{module}Dto.cs");

            File.WriteAllText(file, $@"
namespace Application.Features.{module}s.DTOs;

public class {module}Dto
{{
    public Guid Id {{ get; set; }}
    public string Nombre {{ get; set; }} = string.Empty;
}}
");
        }

        private void GenerateQuery(string path, string module, string action)
        {
            var queryPath = Path.Combine(path, "Queries", $"Get{module}s");
            Directory.CreateDirectory(queryPath);

            var file = Path.Combine(queryPath, $"Get{module}sQuery.cs");

            File.WriteAllText(file, $@"
using MediatR;
using Application.Features.{module}s.DTOs;

namespace Application.Features.{module}s.Queries;

public record Get{module}sQuery() : IRequest<List<{module}Dto>>;
");
        }

        private void GenerateHandler(string path, string module, string action)
        {
            var handlerPath = Path.Combine(path, "Queries", $"Get{module}s");
            var file = Path.Combine(handlerPath, $"Get{module}sHandler.cs");

            File.WriteAllText(file, $@"
using MediatR;
using Application.Features.{module}s.DTOs;

namespace Application.Features.{module}s.Queries;

public class Get{module}sHandler : IRequestHandler<Get{module}sQuery, List<{module}Dto>>
{{
    public async Task<List<{module}Dto>> Handle(Get{module}sQuery request, CancellationToken cancellationToken)
    {{
        return new List<{module}Dto>();
    }}
}}
");
        }

        private void GenerateEndpoint(string path, string module, string action)
        {
            var file = Path.Combine(path, $"{module}sEndpoints.cs");

            File.WriteAllText(file, $@"
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Api.Endpoints;

public static class {module}sEndpoints
{{
    public static void Map{module}sEndpoints(this IEndpointRouteBuilder app)
    {{
        app.MapGet(""/api/{module.ToLower()}s"", async (IMediator mediator) =>
        {{
            return await mediator.Send(new Get{module}sQuery());
        }});
    }}
}}
");
        }
    }
}
