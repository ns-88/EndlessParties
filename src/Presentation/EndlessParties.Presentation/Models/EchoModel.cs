namespace EndlessParties.Presentation.Models;

/// <summary>
/// Модель данных для выполнения эхо-запроса
/// </summary>
public class EchoModel
{
    /// <summary>
    /// Текстовое значение
    /// </summary>
    public required string StringValue { get; init; }

    /// <summary>
    /// Числовое значение
    /// </summary>
    public required int IntValue { get; init; }
}