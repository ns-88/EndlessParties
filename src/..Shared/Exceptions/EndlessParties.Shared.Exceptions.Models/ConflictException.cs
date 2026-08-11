namespace EndlessParties.Shared.Exceptions.Models;

/// <summary>
/// Исключение, возникающее при нарушении бизнес-правил из-за конфликта состояния данных
/// </summary>
public class ConflictException : Exception
{
    /// <inheritdoc />
    public ConflictException(string message) : base(message)
    {
    }
}