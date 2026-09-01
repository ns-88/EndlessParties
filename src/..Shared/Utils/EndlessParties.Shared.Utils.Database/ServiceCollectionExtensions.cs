using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Database.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Shared.Utils.Database;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления контекста базы данных
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление базы данных
    /// </summary>
    public static IServiceCollection AddDatabase<TContext>(this IServiceCollection services,
        DatabaseSettings settings, bool usePooling = true, bool ignorePendingModelChanges = false)
        where TContext : DbContext
    {
        if (usePooling)
        {
            services.AddDbContextPool<TContext>(ConfigureContext);
        }
        else
        {
            services.AddDbContext<TContext>(ConfigureContext);
        }

        services.AddScoped<IUnitOfWork, DefaultUnitOfWork<TContext>>();

        return services;

        void ConfigureContext(DbContextOptionsBuilder dbContextSetup)
        {
            dbContextSetup.UseNpgsql(settings.ConnectionString, npgsqlSetup =>
            {
                npgsqlSetup
                    .EnableRetryOnFailure(settings.RetryReconnectDatabaseCount)
                    .MigrationsAssembly(typeof(TContext).Assembly.FullName);
            });

            if (ignorePendingModelChanges)
            {
                dbContextSetup.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            }

            dbContextSetup.UseSnakeCaseNamingConvention();
        }
    }
}