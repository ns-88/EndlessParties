namespace EndlessParties.Shared.Contracts.Cache;

/// <summary>
/// Ключи кэширования для событий
/// </summary>
public static class EventsKeys
{
    /// <summary>
    /// Одиночное событие
    /// </summary>
    public const string Single = "events:{0}";

    /// <summary>
    /// 10 самых популярных событий
    /// </summary>
    public const string Top10MostFilled = "events:top10";
}