using EndlessParties.Shared.Utils.Database.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EndlessParties.Shared.Utils.Database;

/// <summary>
/// Реализация <see cref="IUnitOfWork"/> для EF Core
/// </summary>
internal class DefaultUnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
{
    /// <summary>
    /// Контекст базы данных
    /// </summary>
    private readonly TContext _dbContext;


    /// <summary>
    /// Конструктор
    /// </summary>
    public DefaultUnitOfWork(TContext dbContext)
    {
        _dbContext = dbContext;
    }


    /// <inheritdoc />
    public Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken)
    {
        return _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IExecutionStrategy CreateExecutionStrategy()
    {
        return _dbContext.Database.CreateExecutionStrategy();
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}