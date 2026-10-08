using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Models;

namespace EndlessParties.Bookings.Api.App.UnitTests;

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
    /// Идентификатор пользователя с ролью "User"
    /// </summary>
    public static readonly Guid OrdinaryUserId = Guid.Parse("a090302b-af1f-4c03-b3b8-1d2b08e4b1bf");

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
    /// Число активных бронирований пользователя
    /// </summary>
    public const int BookingsActiveCount = 5;

    /// <summary>
    /// Контекст пользователя с ролью "Admin"
    /// </summary>
    public static UserContext AdminUserContext => new()
    {
        Id = AdminUserId,
        Name = "Admin",
        Role = UserRole.Admin
    };

    /// <summary>
    /// Контекст пользователя с ролью "User"
    /// </summary>
    public static UserContext OrdinaryUserContext => new()
    {
        Id = OrdinaryUserId,
        Name = "User",
        Role = UserRole.User
    };
}