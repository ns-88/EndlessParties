using System;
using EndlessParties.Shared.Utils.DateTime.Abstractions;

namespace EndlessParties.Shared.Utils.DateTime;

/// <inheritdoc />
public class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow()
    {
        return DateTimeOffset.UtcNow;
    }
}