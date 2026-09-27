using System;

namespace EndlessParties.Shared.Utils.DateTime.Abstractions;

/// <summary>
/// Провайдер данных о времени
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Получение текущего времени в формате UTC
    /// </summary>
    DateTimeOffset UtcNow();
}