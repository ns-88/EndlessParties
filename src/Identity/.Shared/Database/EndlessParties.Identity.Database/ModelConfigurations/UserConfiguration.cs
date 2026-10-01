using EndlessParties.Identity.Domain.Models;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EndlessParties.Identity.Database.ModelConfigurations;

/// <summary>
/// Конфигурация сущности <see cref="User"/>
/// </summary>
internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .ToTable("users");

        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .HasIndex(x => x.Name)
            .IsUnique();
        builder
            .Property(x => x.Name)
            .HasMaxLength(User.MaxNameLength)
            .IsRequired();

        builder
            .Property(x => x.Role)
            .HasConversion(
                toDb => (int)toDb,
                fromDb => (UserRole)fromDb)
            .IsRequired();

        builder
            .OwnsOne(x => x.Password, navBuilder =>
            {
                navBuilder
                    .Property(p => p.Hash)
                    .HasColumnName("password_hash")
                    .HasMaxLength(UserPassword.MaxHashLength)
                    .IsRequired();

                navBuilder
                    .Property(p => p.Salt)
                    .HasColumnName("password_salt")
                    .HasMaxLength(UserPassword.MaxSaltLength)
                    .IsRequired();
            });
    }
}