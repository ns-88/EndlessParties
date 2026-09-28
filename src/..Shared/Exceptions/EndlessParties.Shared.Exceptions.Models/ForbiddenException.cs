namespace EndlessParties.Shared.Exceptions.Models;

/// <summary>
/// Исключение, возникающее в случае запрета пользователю определенных действий
/// </summary>
public class ForbiddenException : Exception
{
    /// <inheritdoc />
    public ForbiddenException(string? message) : base(message)
    {
    }
}