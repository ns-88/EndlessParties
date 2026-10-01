using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Models;

namespace EndlessParties.Events.Api.App.IntegrationTests;

/// <summary>
/// Константы для теста
/// </summary>
internal static class TestConstants
{
    /// <summary>
    /// Идентификатор пользователя с ролью "Admin"
    /// </summary>
    public static readonly Guid AdminUserId = Guid.Parse("d1fabb5a-2238-4887-b6b7-f9ff61daa487");

    /// <summary>
    /// Дата и время начала события
    /// </summary>
    public static readonly DateTimeOffset StartAt = new(new DateTime(2025, 01, 01), TimeSpan.Zero);

    /// <summary>
    /// Дата и время завершения события
    /// </summary>
    public static readonly DateTimeOffset EndAt = new(new DateTime(2025, 01, 02), TimeSpan.Zero);

    /// <summary>
    /// Общее количество мест
    /// </summary>
    public const int TotalSeats = 3;

    /// <summary>
    /// Контекст пользователя с ролью "Admin"
    /// </summary>
    public static UserContext AdminUserContext => new()
    {
        Id = AdminUserId,
        Name = "Admin",
        Role = UserRole.Admin
    };
}