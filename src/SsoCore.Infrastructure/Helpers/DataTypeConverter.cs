using System.Reflection;

namespace SsoCore.Infrastructure.Helpers
{

    public static class DataTypeConverter
    {
        public static Dictionary<string, string> GetFieldsAsDictionary(Type type)
        {
            var dictionary = new Dictionary<string, string>();

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsClass || !type.IsAbstract || !type.IsSealed)
                throw new ArgumentException("The specified type must be a static class.", nameof(type));

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.GetValue(null) is string value)
                {
                    dictionary.Add(value, field.Name);
                }
            }

            return dictionary;
        }
    }
}
