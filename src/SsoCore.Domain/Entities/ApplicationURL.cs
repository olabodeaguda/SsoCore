using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsoCore.Domain.Enums;

namespace SsoCore.Domain.Entities
{
    public class ApplicationURL
    {
        public long Id { get; set; }
        public string? ApplicationId { get; set; }
        public string? URL { get; set; }
        public ApplicationURLType ApplicationURLType { get; set; }
    }
}
