using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace EndlessParties.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleAndDescriptionSearchVectors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_events_search_vector",
                table: "events");

            migrationBuilder.DropColumn(
                name: "search_vector",
                table: "events");

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "description_search_vector",
                table: "events",
                type: "tsvector",
                nullable: true,
                computedColumnSql: "to_tsvector('russian', coalesce(description, ''))",
                stored: true);

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "title_search_vector",
                table: "events",
                type: "tsvector",
                nullable: true,
                computedColumnSql: "to_tsvector('russian', coalesce(title, ''))",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "ix_events_description_search_vector",
                table: "events",
                column: "description_search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_events_title_search_vector",
                table: "events",
                column: "title_search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_events_description_search_vector",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_title_search_vector",
                table: "events");

            migrationBuilder.DropColumn(
                name: "description_search_vector",
                table: "events");

            migrationBuilder.DropColumn(
                name: "title_search_vector",
                table: "events");

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                table: "events",
                type: "tsvector",
                nullable: true,
                computedColumnSql: "to_tsvector('russian', coalesce(title, '') || ' ' || coalesce(description, ''))",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "ix_events_search_vector",
                table: "events",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }
    }
}
