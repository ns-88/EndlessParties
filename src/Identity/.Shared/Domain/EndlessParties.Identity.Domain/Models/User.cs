using EndlessParties.Identity.Domain.Errors;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;

namespace EndlessParties.Identity.Domain.Models;

/// <summary>
/// Пользователь
/// </summary>
public class User
{
    /// <summary>
    /// Максимальная длина имени (логина)
    /// </summary>
    public const int MaxNameLength = 30;


    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Имя (логин)
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Роль
    /// </summary>
    public UserRole Role { get; }

    /// <summary>
    /// Пароль
    /// </summary>
    public UserPassword Password { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    private User()
    {
        Name = string.Empty;
        Role = default;
        Password = null!;
    }

    /// <summary>
    /// Конструктор
    /// </summary>
    public User(string name, UserRole role, UserPassword password)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new LogicException(ApplicationErrors.UserErrors.NameNotSpecified);
        }

        if (role == default || !Enum.IsDefined(role))
        {
            throw new LogicException(ApplicationErrors.UserErrors.RoleHasWrongValue);
        }

        if (password == null!)
        {
            throw new LogicException(ApplicationErrors.UserErrors.PasswordNotSpecified);
        }

        Id = Guid.NewGuid();
        Name = name;
        Role = role;
        Password = password;
    }
}