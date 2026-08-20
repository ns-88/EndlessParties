using EndlessParties.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace EndlessParties.Database.ModelConfigurations;

/// <summary>
/// Конфигурация сущности <see cref="Event"/>
/// </summary>
internal class EventConfiguration : IEntityTypeConfiguration<Event>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder
            .ToTable("events");
        
        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .Property(x => x.Title)
            .HasMaxLength(Event.MaxTitleLength)
            .IsRequired();

        builder
            .Property(x => x.Description)
            .HasMaxLength(Event.MaxDescriptionLength)
            .IsRequired(false);

        builder
            .Property<NpgsqlTsVector>("title_search_vector")
            .HasComputedColumnSql("to_tsvector('russian', coalesce(title, ''))", stored: true);
        builder
            .HasIndex("title_search_vector")
            .HasMethod("GIN");

        builder
            .Property<NpgsqlTsVector>("description_search_vector")
            .HasComputedColumnSql("to_tsvector('russian', coalesce(description, ''))", stored: true);
        builder
            .HasIndex("description_search_vector")
            .HasMethod("GIN");

        builder
            .Property(x => x.TotalSeats)
            .IsRequired();

        builder
            .Property(x => x.AvailableSeats)
            .IsRequired();

        builder
            .HasIndex(x => x.StartAt);
        builder
            .Property(x => x.StartAt)
            .IsRequired();

        builder
            .HasIndex(x => x.EndAt);
        builder
            .Property(x => x.EndAt)
            .IsRequired();

        builder
            .Property(x => x.RowVersion)
            .IsRowVersion();

        builder
            .ToTable(x => x.HasCheckConstraint("ck_events_seats",
                "available_seats >= 0 AND total_seats >= 0 AND available_seats <= total_seats"));

        builder
            .ToTable(x => x.HasCheckConstraint("ck_events_dates",
                "end_at >= start_at"));
    }
}