using EndlessParties.Domain.Errors;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Domain.Models;

public partial class Event
{
    /// <summary>
    /// Валидация <see cref="Title"/>
    /// </summary>
    private static string ValidateTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new LogicException(ApplicationErrors.Events.NameNotSpecified);
        }

        if (value.Length > MaxTitleLength)
        {
            throw new LogicException(ApplicationErrors.Events.NameLongerThanAllowed);
        }

        return value;
    }

    /// <summary>
    /// Валидация <see cref="TotalSeats"/>
    /// </summary>
    private static int ValidateTotalSeats(int value)
    {
        if (value <= 0)
        {
            throw new LogicException(ApplicationErrors.Events.TotalSeatsLessAllowed);
        }

        return value;
    }

    /// <summary>
    /// Валидация <see cref="Description"/>
    /// </summary>
    private static string? ValidateDescription(string? value)
    {
        if (value is { Length: > MaxDescriptionLength })
        {
            throw new LogicException(ApplicationErrors.Events.DescriptionLongerThanAllowed);
        }

        return value;
    }

    /// <summary>
    /// Валидация <see cref="StartAt"/> и <see cref="EndAt"/>
    /// </summary>
    private static void ValidateStartAndEndAt(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        if (startAt == default)
        {
            throw new LogicException(ApplicationErrors.Events.DateAndTimeStartNotSet);
        }

        if (endAt == default)
        {
            throw new LogicException(ApplicationErrors.Events.DateAndTimeCompletionNotSet);
        }

        if (startAt >= endAt)
        {
            throw new LogicException(ApplicationErrors.Events.StartCannotLaterCompletion);
        }
    }
}