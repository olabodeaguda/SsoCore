using FluentValidation;
using System.Linq.Expressions;

namespace SsoCore.Application.Validations
{
    public class ValidationHelper<T> : AbstractValidator<T> where T : class
    {
        public void ValidateNotEmpty(Expression<Func<T, object?>> propertyExpression, string? propertyName = null)
        {
            var actualPropertyName = string.IsNullOrEmpty(propertyName)
                               ? GetPropertyName(propertyExpression)
                               : propertyName;

            RuleFor(propertyExpression)
                .NotEmpty().WithMessage($"{actualPropertyName} is required")
                .NotNull().WithMessage($"{actualPropertyName} is required")
                .OverridePropertyName(actualPropertyName);
        }
        public void ValidateListNotEmpty<F>(Expression<Func<T, ICollection<F>?>> propertyExpression, string? propertyName = null)
        {
            var actualPropertyName = string.IsNullOrEmpty(propertyName)
                                ? GetPropertyName(propertyExpression)
                                : propertyName;

            RuleFor(propertyExpression)
                .Must(collection => collection != null && collection.Any())
                .WithMessage($"{actualPropertyName} is required and cannot be empty.");
        }

        private string GetPropertyName(Expression<Func<T, object?>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            throw new InvalidOperationException("Expression is not a member expression");
        }

        private string GetPropertyName<F>(Expression<Func<T, ICollection<F>?>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name; 
            }

            throw new InvalidOperationException("Expression is not a member expression");
        }
    }
}
