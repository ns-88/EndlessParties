using FluentValidation;

namespace EndlessParties.Identity.Api.App.Features.Login;

/// <summary>
/// Валидатор <see cref="LoginUserRequest"/>
/// </summary>
internal class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    /// <inheritdoc />
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}