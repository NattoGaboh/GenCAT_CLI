using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCAT_CLI.Models
{
    public class ProjectOptions
    {
        public string Name { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public bool UseJwt { get; set; }
        public bool MultiTenant { get; set; }
    }
}
