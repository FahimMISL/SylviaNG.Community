using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Community.Migrations
{
    /// <inheritdoc />
    public partial class RemoveElectionCandidateIsApproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "ElectionCandidates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // defaultValue: true (not false) - if this migration is ever rolled back, existing
            // candidates should not retroactively look unapproved; every candidate nominated
            // after this feature shipped is effectively "always approved".
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "ElectionCandidates",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }
    }
}
