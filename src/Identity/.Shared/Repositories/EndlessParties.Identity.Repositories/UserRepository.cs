using EndlessParties.Identity.Database.Database;
using EndlessParties.Identity.Domain.Errors;
using EndlessParties.Identity.Domain.Models;
using EndlessParties.Identity.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Identity.Repositories;

/// <inheritdoc />
internal class UserRepository : IUserRepository
{
    /// <summary>
    /// Таблица <see cref="User"/>
    /// </summary>
    private readonly DbSet<User> _users;


    /// <summary>
    /// Конструктор
    /// </summary>
    public UserRepository(IdentityDbContext dbContext)
    {
        _users = dbContext.Users;
    }


    /// <inheritdoc />
    public Task Create(User model, CancellationToken cancellationToken)
    {
        _users.Add(model);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<User> GetByName(string name, CancellationToken cancellationToken)
    {
        User? user;

        try
        {
            user = await _users.SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new LogicException(string.Format(ApplicationErrors.UserErrors.ReceivingByName, name), ex);
        }

        if (user == null)
        {
            throw new NotFoundException(string.Format(ApplicationErrors.UserErrors.NotFound, name));
        }

        return user;
    }

    /// <inheritdoc />
    public async Task<bool> Exists(string name, CancellationToken cancellationToken)
    {
        bool exists;

        try
        {
            exists = await _users.AnyAsync(x => x.Name == name, cancellationToken);
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.UserErrors.ReceivingByName, name), ex);
        }

        return exists;
    }
}