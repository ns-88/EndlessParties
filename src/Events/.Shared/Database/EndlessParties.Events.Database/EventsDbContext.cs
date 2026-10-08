using EndlessParties.Events.Database.ModelConfigurations;
using EndlessParties.Events.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Events.Database;

/// <summary>
/// Контекст базы данных "Events" - хранение и управление данными мероприятий (событий)
/// </summary>
public class EventsDbContext : DbContext
{
    /// <summary>
    /// Таблица мероприятий (событий)
    /// </summary>
    public DbSet<Event> Events => Set<Event>();


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventsDbContext(DbContextOptions<EventsDbContext> options) : base(options)
    {
    }


    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventConfiguration());
    }
}