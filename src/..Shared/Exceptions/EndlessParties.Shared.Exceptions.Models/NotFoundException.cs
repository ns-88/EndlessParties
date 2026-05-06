using System;

namespace EndlessParties.Shared.Exceptions.Models;

/// <summary>
/// Исключение, возникающее при отсутствии запрашиваемого объекта
/// </summary>
public class NotFoundException : Exception
{
    /// <inheritdoc />
    public NotFoundException(string message) : base(message)
    {
    }
}