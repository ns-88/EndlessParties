using DatabaseSchemaReader.DataSchema;
using EndlessParties.Database.Database;
using EndlessParties.IntegrationTests.Infrastructure.Fixtures;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Xunit;

namespace EndlessParties.IntegrationTests.Infrastructure.Migrations;

/// <summary>
/// Тесты схемы базы данных
/// </summary>
public class DatabaseSchemaTests : BaseIntegrationTest<EventsDbContext, DatabaseSchemaFixture>
{
    /// <inheritdoc />
    public DatabaseSchemaTests(DatabaseSchemaFixture fixture) : base(fixture)
    {
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : DatabaseSchemaTests
    {
        /// <inheritdoc />
        public Positive(DatabaseSchemaFixture fixture) : base(fixture)
        {
        }


        /// <summary>
        /// Схема базы данных после миграций соответствует ожидаемой структуре
        /// </summary>
        [Theory]
        [ClassData(typeof(DatabaseSchemaTestData))]
        public async Task DatabaseSchema_AfterMigrations_MatchesExpectedStructure(DatabaseSchemaTestCase testCase)
        {
            // #### Arrange ####
            var schema = await Fixture.GetSchema(TestCancellationToken);

            // #### Act ####
            var table = schema.Tables.FirstOrDefault(x => x.Name == testCase.TableName);

            // #### Assert ####
            table.Should().NotBeNull();

            table.Columns.Should().BeEquivalentTo(testCase.Columns, options => options
                .ComparingByMembers<ColumnData>()
                .WithoutStrictOrdering());

            table.Indexes.Should().BeEquivalentTo(testCase.Indexes, options => options
                .ComparingByMembers<IndexData>()
                .ComparingByMembers<ColumnData>()
                .WithoutStrictOrdering());

            table.ForeignKeys.Should().BeEquivalentTo(testCase.ForeignKeys, options => options
                .ComparingByMembers<ForeignKeyData>()
                .WithoutStrictOrdering());

            table.CheckConstraints.Should().BeEquivalentTo(testCase.Constraints, options => options
                .ComparingByMembers<ConstraintData>()
                .WithoutStrictOrdering());
        }
    }
}