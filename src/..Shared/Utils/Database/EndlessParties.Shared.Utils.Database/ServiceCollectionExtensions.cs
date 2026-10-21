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
    public static IServiceCollection AddDatabase<TContext>(this IServiceCollection services, DatabaseSettings settings,
        bool usePooling = true, bool ignorePendingModelChanges = false, bool useDomainEvents = false)
        where TContext : DbContext
    {
        settings.Validate();

        services.AddScoped<IUnitOfWork, DefaultUnitOfWork<TContext>>();

        if (useDomainEvents)
        {
            services.AddSingleton<DomainEventsInterceptor>();
        }

        if (usePooling)
        {
            services.AddDbContextPool<TContext>(ConfigureContext);
        }
        else
        {
            services.AddDbContext<TContext>(ConfigureContext);
        }

        return services;

        void ConfigureContext(IServiceProvider serviceProvider, DbContextOptionsBuilder dbContextSetup)
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

            if (useDomainEvents)
            {
                dbContextSetup.AddInterceptors(serviceProvider.GetRequiredService<DomainEventsInterceptor>());
            }
        }
    }
}