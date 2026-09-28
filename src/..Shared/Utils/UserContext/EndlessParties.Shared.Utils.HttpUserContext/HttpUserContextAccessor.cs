using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;
using Microsoft.AspNetCore.Http;

namespace EndlessParties.Shared.Utils.HttpUserContext;

using UserContext = UserContext.Abstractions.Models.UserContext;

/// <summary>
/// Сервис для доступа к HTTP-контексту пользователя
/// </summary>
internal class HttpUserContextAccessor : IUserContextAccessor
{
    /// <summary>
    /// Сервис для доступа к HTTP-контексту пользователя
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;


    /// <summary>
    /// Конструктор
    /// </summary>
    public HttpUserContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    /// <inheritdoc />
    public UserContext Current
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User.Identity?.IsAuthenticated != true)
            {
                throw new LogicException("Отсутствует аутентификация пользователя");
            }

            var sub = httpContext.User.FindFirst("sub");
            if (!Guid.TryParse(sub?.Value, out var userId))
            {
                throw new LogicException("Ошибка получения идентификатора пользователя");
            }

            var userName = httpContext.User.FindFirst("name")?.Value;
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new LogicException("Ошибка получения имени пользователя");
            }

            var role = httpContext.User.FindFirst("role");
            if (!Enum.TryParse<UserRole>(role?.Value, out var userRole))
            {
                throw new LogicException("Ошибка получения роли пользователя");
            }

            return new UserContext
            {
                Id = userId,
                Name = userName,
                Role = userRole
            };
        }
    }
}