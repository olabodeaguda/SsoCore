using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsoCore.Application.Configurations
{
    public class SSOSettings
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
