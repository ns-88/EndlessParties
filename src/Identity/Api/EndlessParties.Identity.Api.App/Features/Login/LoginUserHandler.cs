using EndlessParties.Identity.Cryptography.Abstractions;
using EndlessParties.Identity.Cryptography.Abstractions.Models;
using EndlessParties.Identity.Domain.Errors;
using EndlessParties.Identity.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using Mediator;

namespace EndlessParties.Identity.Api.App.Features.Login;

/// <summary>
/// Обработчик <see cref="LoginUserQuery"/>
/// </summary>
public class LoginUserHandler : IRequestHandler<LoginUserQuery, LoginUserResponse>
{
    /// <summary>
    /// Менеджер <see cref="IPasswordManager"/>
    /// </summary>
    private readonly IPasswordManager _passwordManager;

    /// <summary>
    /// Репозиторий <see cref="IUserRepository"/>
    /// </summary>
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Генератор <see cref="IJwtTokenGenerator"/>
    /// </summary>
    private readonly IJwtTokenGenerator _jwtTokenGenerator;


    /// <summary>
    /// Конструктор
    /// </summary>
    public LoginUserHandler(
        IPasswordManager passwordManager,
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _passwordManager = passwordManager;
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }


    /// <inheritdoc />
    public async ValueTask<LoginUserResponse> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        LoginUserResponse loginUserResponse;
        var loginRequest = request.LoginRequest;

        try
        {
            var user = await _userRepository.GetByName(loginRequest.Name, cancellationToken);
            var passwordData = new PasswordModel(user.Password.Hash, user.Password.Salt);
            var isValid = _passwordManager.ValidatePassword(loginRequest.Password, passwordData);

            if (!isValid)
            {
                throw new LogicException(ApplicationErrors.UserPasswordErrors.IncorrectCredentials);
            }

            var jwtToken = _jwtTokenGenerator.Generate(user);

            loginUserResponse = new LoginUserResponse
            {
                UserId = user.Id,
                UserRole = user.Role,
                AccessToken = jwtToken.Token,
                ExpiresIn = jwtToken.Expires
            };
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new LogicException(string.Format(ApplicationErrors.UserErrors.Login, loginRequest.Name), ex);
        }

        return loginUserResponse;
    }
}