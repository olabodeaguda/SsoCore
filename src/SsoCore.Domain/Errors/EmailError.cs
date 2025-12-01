using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsoCore.Domain.Common;

namespace SsoCore.Domain.Errors
{
    public class EmailError
    {
        public static Error TemplateNotFound(string message) => new("EMAIL_TEMPLATE_NOT_FOUND", message ?? "Email template not found");
    }
}
