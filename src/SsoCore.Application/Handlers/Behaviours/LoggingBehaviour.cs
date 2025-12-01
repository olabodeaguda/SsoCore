using MediatR;
using Microsoft.Extensions.Logging;

namespace SsoCore.Application.Handlers.Behaviours
{
    public class LoggingBehaviour<TRequest, TRespoonse>(ILogger<LoggingBehaviour<TRequest, TRespoonse>> logger)
    : IPipelineBehavior<TRequest, TRespoonse> where TRequest : notnull
    {
        private ILogger<LoggingBehaviour<TRequest, TRespoonse>> Logger { get; set;  } = logger;

        public async Task<TRespoonse> Handle(TRequest request, RequestHandlerDelegate<TRespoonse> next, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Handling request {@Request}", request);

            TRespoonse? response = default(TRespoonse);
            try
            {
                response = await next();
            }
            finally
            {
                Logger.LogInformation("Handling response {@Response}", response);
            }
            return response;
        }
    }
}
