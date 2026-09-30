using EndlessParties.Identity.Domain.Models;
using FluentValidation;

namespace EndlessParties.Identity.Api.App.Features.Register;

/// <summary>
/// Валидатор <see cref="RegisterUserRequest"/>
/// </summary>
internal class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    /// <inheritdoc />
    public RegisterUserRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(User.MaxNameLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(UserPassword.MaxLength);
    }
}