using Microsoft.EntityFrameworkCore.Storage;

namespace EndlessParties.Shared.Utils.Database.Abstractions;

/// <summary>
/// Логическая единица работы с базой данных, инкапсулирующая создание транзакций и сохранение изменений данных
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Получение транзакции с уровнем изоляции по умолчанию
    /// </summary>
    Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken);

    /// <summary>
    /// Получение политики повторов
    /// </summary>
    IExecutionStrategy CreateExecutionStrategy();

    /// <summary>
    /// Сохранение произведенных изменений в данных
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}