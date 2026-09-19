using EndlessParties.Identity.Database.Database;
using EndlessParties.Shared.Utils.Database.Migrations;

namespace EndlessParties.Identity.Database;

/// <inheritdoc />
internal class MigrationFactory : MigrationFactoryBase<IdentityDbContext>
{
    /// <inheritdoc />
    public MigrationFactory() : base("Postgres:Identity")
    {
    }
}