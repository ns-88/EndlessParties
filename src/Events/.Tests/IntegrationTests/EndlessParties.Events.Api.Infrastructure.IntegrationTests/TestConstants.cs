namespace EndlessParties.Events.Api.Infrastructure.IntegrationTests;

/// <summary>
/// Константы для теста
/// </summary>
internal static class TestConstants
{
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
}