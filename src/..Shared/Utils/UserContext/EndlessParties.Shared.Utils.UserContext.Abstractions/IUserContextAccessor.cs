namespace EndlessParties.Shared.Utils.UserContext.Abstractions;

using UserContext = Models.UserContext;

/// <summary>
/// Сервис для доступа к контексту пользователя
/// </summary>
public interface IUserContextAccessor
{
    /// <summary>
    /// Контекст текущего пользователя
    /// </summary>
    UserContext Current { get; }
}