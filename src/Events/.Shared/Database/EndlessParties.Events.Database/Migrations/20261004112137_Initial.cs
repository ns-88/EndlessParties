using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace EndlessParties.Events.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    total_seats = table.Column<int>(type: "integer", nullable: false),
                    available_seats = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    description_search_vector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: true, computedColumnSql: "to_tsvector('russian', coalesce(description, ''))", stored: true),
                    title_search_vector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: true, computedColumnSql: "to_tsvector('russian', coalesce(title, ''))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events", x => x.id);
                    table.CheckConstraint("ck_events_dates", "end_at >= start_at");
                    table.CheckConstraint("ck_events_seats", "available_seats >= 0 AND total_seats >= 0 AND available_seats <= total_seats");
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events");
        }
    }
}
