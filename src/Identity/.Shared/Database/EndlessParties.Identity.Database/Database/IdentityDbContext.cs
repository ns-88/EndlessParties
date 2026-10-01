using EndlessParties.Identity.Database.ModelConfigurations;
using EndlessParties.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Identity.Database.Database;

/// <summary>
/// Контекст базы данных "Identity" - хранение учетных записей пользователей и данных аутентификации
/// </summary>
public class IdentityDbContext : DbContext
{
    /// <summary>
    /// Таблица пользователей
    /// </summary>
    public DbSet<User> Users => Set<User>();


    /// <summary>
    /// Конструктор
    /// </summary>
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }


    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}