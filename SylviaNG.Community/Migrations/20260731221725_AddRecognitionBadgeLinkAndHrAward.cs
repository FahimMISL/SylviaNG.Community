using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Community.Migrations
{
    /// <inheritdoc />
    public partial class AddRecognitionBadgeLinkAndHrAward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BadgeId",
                table: "Recognitions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHrIssued",
                table: "Recognitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Badges",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Badges",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recognitions_BadgeId",
                table: "Recognitions",
                column: "BadgeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recognitions_Badges_BadgeId",
                table: "Recognitions",
                column: "BadgeId",
                principalTable: "Badges",
                principalColumn: "BadgeId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recognitions_Badges_BadgeId",
                table: "Recognitions");

            migrationBuilder.DropIndex(
                name: "IX_Recognitions_BadgeId",
                table: "Recognitions");

            migrationBuilder.DropColumn(
                name: "BadgeId",
                table: "Recognitions");

            migrationBuilder.DropColumn(
                name: "IsHrIssued",
                table: "Recognitions");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Badges");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Badges");
        }
    }
}
