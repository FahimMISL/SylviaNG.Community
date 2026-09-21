using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Community.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePathToChatMessageAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "ChatMessageAttachments",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            // Backfill existing rows from the FileStorage row they already reference, instead
            // of leaving them at the "" scaffolded default above.
            migrationBuilder.Sql(
                """
                UPDATE "ChatMessageAttachments" AS cma
                SET "FilePath" = fs."StoragePath"
                FROM "FileStorages" AS fs
                WHERE fs."FileId" = cma."FileStorageId";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "ChatMessageAttachments");
        }
    }
}
