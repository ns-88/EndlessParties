using EndlessParties.Bookings.Domain.Enums;
using EndlessParties.Bookings.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EndlessParties.Bookings.Database.ModelConfigurations;

/// <summary>
/// Конфигурация сущности <see cref="Booking"/>
/// </summary>
internal class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder
            .ToTable("bookings");

        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .HasIndex(x => x.EventId);
        builder
            .Property(x => x.EventId)
            .IsRequired();

        builder
            .HasIndex(x => x.UserId);
        builder
            .Property(x => x.UserId)
            .IsRequired();

        builder
            .Property(x => x.Status)
            .HasConversion(
                toDb => (int)toDb,
                fromDb => (BookingStatus)fromDb)
            .IsRequired();

        builder
            .HasIndex(x => x.CreatedAt);
        builder
            .Property(x => x.CreatedAt)
            .IsRequired();

        builder
            .HasIndex(x => x.ProcessedAt);
        builder
            .Property(x => x.ProcessedAt)
            .IsRequired(false);
    }
}