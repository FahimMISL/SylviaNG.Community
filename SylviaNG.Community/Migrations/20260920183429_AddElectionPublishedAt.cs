using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Community.Migrations
{
    /// <inheritdoc />
    public partial class AddElectionPublishedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                table: "Elections",
                type: "timestamp with time zone",
                nullable: true);

            // Backfill already-published elections (anything past Draft) so the newest-published-first
            // ordering is right for existing rows: the best available proxy is their last update.
            migrationBuilder.Sql(
                "UPDATE \"Elections\" SET \"PublishedAt\" = COALESCE(\"UpdatedAt\", \"CreatedAt\") WHERE \"Status\" <> 'Draft';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishedAt",
                table: "Elections");
        }
    }
}
