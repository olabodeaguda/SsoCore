using System.Net;
using SsoCore.Domain.Common;

namespace SsoCore.Provider.Helper
{
    public static class ResultExtension
    {
        public static IResult Problem(this Result result,
           HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return Results.Problem(
                detail: result.Message ?? "An error occur",
                statusCode: (int)statusCode,
                title: "Error",
                extensions: new Dictionary<string, object?>
                            {
                                { "Error", result.Error }
                            }
            );
        }
    }
}
