using OpenIddict.Abstractions;

namespace SsoCore.Infrastructure.Helpers
{
    public class OpenIdDictModelWrapperConstants
    {
        public static Dictionary<string, string> ClientTypes() =>
            DataTypeConverter.GetFieldsAsDictionary(typeof(OpenIddictConstants.ClientTypes));

        public static Dictionary<string, string> ConsentTypes() =>
            DataTypeConverter.GetFieldsAsDictionary(typeof(OpenIddictConstants.ConsentTypes));

        public static Dictionary<string, string> ApplicationTypes() =>
            DataTypeConverter.GetFieldsAsDictionary(typeof(OpenIddictConstants.ApplicationTypes));

        public static Dictionary<string, string> GrantTypes() =>
            DataTypeConverter.GetFieldsAsDictionary(typeof(OpenIddictConstants.Permissions.GrantTypes));
        public static Dictionary<string, string> ResponseTypes() =>
            DataTypeConverter.GetFieldsAsDictionary(typeof(OpenIddictConstants.Permissions.ResponseTypes));
        public static Dictionary<string, string> DefaultScopes() =>
            DataTypeConverter.GetFieldsAsDictionary(typeof(OpenIddictConstants.Permissions.Scopes));
    }
}
