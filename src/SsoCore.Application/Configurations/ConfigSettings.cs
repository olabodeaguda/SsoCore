using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsoCore.Application.Configurations
{
    public class ConfigSettings
    {
        private IConfiguration _configuration { get; }
        public ConfigSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public SSOSettings SSOSettings => _configuration.GetSection("SSOSettings").Get<SSOSettings>()!;
    }
}
