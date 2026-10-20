using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EndlessParties.Events.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_events_description_search_vector",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_end_at",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_start_at",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_title_search_vector",
                table: "events");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "events",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_events_available_seats_total_seats",
                table: "events",
                columns: new[] { "available_seats", "total_seats" },
                filter: "is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_events_description_search_vector",
                table: "events",
                column: "description_search_vector",
                filter: "is_deleted = FALSE")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_events_end_at",
                table: "events",
                column: "end_at",
                filter: "is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_events_start_at",
                table: "events",
                column: "start_at",
                filter: "is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_events_title_search_vector",
                table: "events",
                column: "title_search_vector",
                filter: "is_deleted = FALSE")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_events_available_seats_total_seats",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_description_search_vector",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_end_at",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_start_at",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_title_search_vector",
                table: "events");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "events");

            migrationBuilder.CreateIndex(
                name: "ix_events_description_search_vector",
                table: "events",
                column: "description_search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_events_end_at",
                table: "events",
                column: "end_at");

            migrationBuilder.CreateIndex(
                name: "ix_events_start_at",
                table: "events",
                column: "start_at");

            migrationBuilder.CreateIndex(
                name: "ix_events_title_search_vector",
                table: "events",
                column: "title_search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }
    }
}
