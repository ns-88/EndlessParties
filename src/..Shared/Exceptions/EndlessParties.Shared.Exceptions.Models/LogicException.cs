using System;

namespace EndlessParties.Shared.Exceptions.Models;

/// <summary>
/// Исключение, возникающее при ошибках логики
/// </summary>
public class LogicException : Exception
{
    /// <inheritdoc />
    public LogicException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public LogicException(string message, Exception innerException) : base(message, innerException)
    {
    }
}