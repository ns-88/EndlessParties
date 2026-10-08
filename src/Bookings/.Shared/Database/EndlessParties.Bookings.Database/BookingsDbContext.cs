using EndlessParties.Bookings.Database.ModelConfigurations;
using EndlessParties.Bookings.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Bookings.Database;

/// <summary>
/// Контекст базы данных "Bookings" - хранение и управление данными бронирований
/// </summary>
public class BookingsDbContext : DbContext
{
    /// <summary>
    /// Таблица бронирований
    /// </summary>
    public DbSet<Booking> Bookings => Set<Booking>();


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingsDbContext(DbContextOptions<BookingsDbContext> options) : base(options)
    {
    }


    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
    }
}