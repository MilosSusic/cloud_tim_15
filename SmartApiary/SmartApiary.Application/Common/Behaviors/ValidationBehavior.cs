using FluentValidation;
using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .Select(f => f.ErrorMessage)
                .Distinct()
                .ToList();

            if (failures.Count == 0)
            {
                return await next();
            }

            var errorMessage = string.Join(" ", failures);

            if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(errorMessage);
            }

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var failureMethod = typeof(TResponse).GetMethod(
                    "Failure",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Static);

                if (failureMethod is not null)
                {
                    var failureResult = failureMethod.Invoke(null, new object[] { errorMessage });
                    if (failureResult is not null)
                    {
                        return (TResponse)failureResult;
                    }
                }
            }

            throw new InvalidOperationException(
                $"ValidationBehavior cannot create failure result for response type {typeof(TResponse).Name}.");
        }
    }
}