using EndlessParties.Database.ModelConfigurations;
using EndlessParties.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Database.Database;

/// <summary>
/// Контекст базы данных "Events"
/// </summary>
public class EventsDbContext : DbContext
{
    /// <summary>
    /// Таблица мероприятий (событий)
    /// </summary>
    public DbSet<Event> Events => Set<Event>();

    /// <summary>
    /// Таблица бронирований
    /// </summary>
    public DbSet<Booking> Bookings => Set<Booking>();


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventsDbContext(DbContextOptions<EventsDbContext> options) : base(options)
    {
    }


    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new EventConfiguration())
            .ApplyConfiguration(new BookingConfiguration());
    }
}