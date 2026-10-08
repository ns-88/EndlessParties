using EndlessParties.Identity.Cryptography.Abstractions;
using EndlessParties.Identity.Domain.Errors;
using EndlessParties.Identity.Domain.Models;
using EndlessParties.Identity.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Extensions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;
using Mediator;

namespace EndlessParties.Identity.Api.App.Features.Register;

/// <summary>
/// Обработчик <see cref="RegisterUserCommand"/>
/// </summary>
public class RegisterUserHandler : IRequestHandler<RegisterUserCommand>
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
    /// Единица работы <see cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;


    /// <summary>
    /// Конструктор
    /// </summary>
    public RegisterUserHandler(
        IPasswordManager passwordManager,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordManager = passwordManager;
        _unitOfWork = unitOfWork;
    }


    /// <inheritdoc />
    public async ValueTask<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var registerRequest = request.RegisterRequest;

        try
        {
            if (await _userRepository.Exists(registerRequest.Name, cancellationToken))
            {
                throw new ConflictException(string.Format(ApplicationErrors.UserErrors.SpecifiedLoginAlreadyExists, registerRequest.Name));
            }

            var passwordData = _passwordManager.CreatePassword(registerRequest.Password);
            var userPassword = new UserPassword(passwordData.Hash, passwordData.Salt);
            var user = new User(registerRequest.Name, UserRole.User, userPassword);

            await _userRepository.Create(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConflictException)
        {
            throw;
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.UserErrors.RegistrationError, registerRequest.Name), ex);
        }

        return Unit.Value;
    }
}