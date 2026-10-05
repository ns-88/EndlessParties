using EndlessParties.Shared.Utils.Database.Migrations;

namespace EndlessParties.Events.Database;

/// <inheritdoc />
internal class MigrationFactory : MigrationFactoryBase<EventsDbContext>
{
    /// <inheritdoc />
    public MigrationFactory() : base("Postgres:Events")
    {
    }
}