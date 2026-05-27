using GenCAT_CLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCAT_CLI.Services
{
    public class PlaceholderService
    {
        public string Replace(string content, ProjectOptions options)
        {
            return content
                .Replace("{{ProjectName}}", options.Name)
                .Replace("{{Database}}", options.Database)
                .Replace("{{UseJwt}}", options.UseJwt.ToString())
                .Replace("{{MultiTenant}}", options.MultiTenant.ToString());
        }
    }
}
