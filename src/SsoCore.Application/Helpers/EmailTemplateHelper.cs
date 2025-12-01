using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;

namespace SsoCore.Application.Helpers
{
    public class EmailTemplateHelper
    {
        private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public static async Task<Result<string>> GetEmailTemplate(string templateName)
        {
            if (_cache.TryGetValue(templateName, out string? cachedTemplate))
            {
                return Result<string>.Success(cachedTemplate!);
            }
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string path = Path.Combine(dir, "EmailTemplates", templateName);
            if (!File.Exists(path))
            {
                return Result<string>.Fail(EmailError.TemplateNotFound($"Email template {templateName} not found"));
            }

            string template = await File.ReadAllTextAsync(path);

            _cache.Set(templateName, template, TimeSpan.FromMinutes(30));

            return Result<string>.Success(template);
        }
    }
}
