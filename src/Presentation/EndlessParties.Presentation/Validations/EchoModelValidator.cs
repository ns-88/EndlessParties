using EndlessParties.Presentation.Models;
using FluentValidation;

namespace EndlessParties.Presentation.Validations;

/// <summary>
/// Валидатор <see cref="EchoModel"/>
/// </summary>
public class EchoModelValidator : AbstractValidator<EchoModel>
{
    /// <inheritdoc />
    public EchoModelValidator()
    {
        RuleFor(x => x.StringValue)
            .NotEmpty();

        RuleFor(x => x.IntValue)
            .GreaterThan(10);
    }
}