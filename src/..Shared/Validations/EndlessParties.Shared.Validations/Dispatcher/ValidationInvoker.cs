using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Shared.Validations.Dispatcher;

/// <inheritdoc />
internal class ValidationInvoker<T> : IValidationInvoker
{
    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(object model, IServiceProvider serviceProvider)
    {
        var validator = serviceProvider.GetService<IValidator<T>>();

        if (validator == null)
        {
            return ValidationResult.Empty;
        }

        var result = await validator.ValidateAsync((T)model);

        return new ValidationResult(result.IsValid, result.Errors);
    }
}