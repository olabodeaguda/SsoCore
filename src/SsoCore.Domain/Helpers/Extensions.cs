using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsoCore.Domain.Helpers
{
    public static class Extensions
    {
        public static string MaskEmail(this string email)
        {
            var atIndex = email.IndexOf('@');
            if (atIndex <= 1)
            {
                return email;
            }

            var maskedEmail = email.Substring(0, 1) + new string('*', atIndex - 1) + email.Substring(atIndex);
            return maskedEmail;
        }
    }
}
