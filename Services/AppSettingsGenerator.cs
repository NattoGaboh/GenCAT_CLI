using GenCAT_CLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCAT_CLI.Services
{
    public class AppSettingsGenerator
    {
        public void Generate(ProjectOptions options, string apiPath)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "appsettings", "appsettings.json.template");

            var content = File.ReadAllText(templatePath);

            var connectionString = GetConnectionString(options);

            content = content
                .Replace("{{ProjectName}}", options.Name)
                .Replace("{{Database}}", options.Database)
                .Replace("{{ConnectionString}}", connectionString)
                .Replace("{{JwtKey}}", GenerateJwtKey())
                .Replace("{{MultiTenant}}", options.MultiTenant.ToString());

            var outputPath = Path.Combine(apiPath, "appsettings.json");

            File.WriteAllText(outputPath, content);
        }

        private string GetConnectionString(ProjectOptions options)
        {
            if (options.Database == "PostgreSQL")
                return "Host=localhost;Port=5432;Database=MyDb;Username=postgres;Password=1234";

            return "Server=localhost;Database=MyDb;User Id=sa;Password=Your_password123;";
        }

        private string GenerateJwtKey()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
