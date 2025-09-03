using System.Diagnostics.Contracts;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Customer.Application.Behavior
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>

    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validators.Any())
                await next();
            var context = new ValidationContext<TRequest>(request);

            var errorDictionary = validators
                .Select(v => v.Validate(context))
                .SelectMany(v => v.Errors)
                .Where(f => f is not null)
                .GroupBy(f => f.PropertyName,
                f => f.ErrorMessage, (propertyName, errorMessage) => new
                {
                    Key = propertyName,
                    Value = errorMessage.Distinct().ToArray()
                }).ToDictionary(f => f.Key, f => f.Value[0]);
            if (errorDictionary.Any())
            {
                var errors = errorDictionary.Select(f => new ValidationFailure
                {
                    PropertyName = f.Value,
                    ErrorCode = f.Key
                }).ToList();

                throw new ValidationException(errors);
            }
            return await next();




        }
    }
}
