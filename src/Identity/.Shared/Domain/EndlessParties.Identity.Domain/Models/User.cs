using EndlessParties.Identity.Domain.Enums;
using EndlessParties.Identity.Domain.Errors;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Identity.Domain.Models;

/// <summary>
/// Пользователь
/// </summary>
public class User
{
    /// <summary>
    /// Максимальная длина имени (логина)
    /// </summary>
    public const int MaxNameLength = 50;


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
            throw new LogicException(ApplicationErrors.UsersErrors.NameNotSpecified);
        }

        if (role == default || !Enum.IsDefined(role))
        {
            throw new LogicException(ApplicationErrors.UsersErrors.RoleHasWrongValue);
        }

        if (password == null!)
        {
            throw new LogicException(ApplicationErrors.UsersErrors.PasswordNotSpecified);
        }

        Id = Guid.NewGuid();
        Name = name;
        Role = role;
        Password = password;
    }
}