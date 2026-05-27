using GenCAT_CLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCAT_CLI.Services
{
    public class FileGeneratorService
    {
        public void Generate(string templatePath, string outputPath, ProjectOptions options)
        {
            foreach (var file in Directory.GetFiles(templatePath, "*", SearchOption.AllDirectories))
            {
                var content = File.ReadAllText(file);

                content = new PlaceholderService().Replace(content, options);

                var newPath = file.Replace(templatePath, outputPath)
                                  .Replace("TemplateProject", options.Name);

                var directory = Path.GetDirectoryName(newPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                File.WriteAllText(newPath, content);
            }
        }
    }
}
