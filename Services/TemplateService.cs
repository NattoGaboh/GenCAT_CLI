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
        public void GenerateProject(ProjectOptions options)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "CleanArchitecture", "TemplateProject");
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), options.Name);

            var generator = new FileGeneratorService();
            generator.Generate(templatePath, outputPath, options);
        }
    }
}
