using FluentValidation;
using MediatR;

namespace SsoCore.Application.Handlers.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any()) return await next();
            var context = new ValidationContext<TRequest>(request);
            var validationResult =
                await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResult.SelectMany(error => error.Errors).Where(_ => _ != null).ToList();
            if (failures.Count != 0)
                throw new ValidationException(failures);

            return await next();
        }
    }
}
