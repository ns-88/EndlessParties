using EndlessParties.Identity.Domain.Models;

namespace EndlessParties.Identity.Repositories.Abstractions;

/// <summary>
/// Репозиторий для работы с пользователями
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Создание пользователя
    /// </summary>
    Task Create(User model, CancellationToken cancellationToken);

    /// <summary>
    /// Получение пользователя по указанному имени (логину)
    /// </summary>
    Task<User> GetByName(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Получение признака наличия пользователя с указанным именем (логином)
    /// </summary>
    Task<bool> Exists(string name, CancellationToken cancellationToken);
}