using EndlessParties.Shared.Utils.Database.Migrations;

namespace EndlessParties.Bookings.Database;

/// <inheritdoc />
internal class MigrationFactory : MigrationFactoryBase<BookingsDbContext>
{
    /// <inheritdoc />
    public MigrationFactory() : base("Postgres:Bookings")
    {
    }
}