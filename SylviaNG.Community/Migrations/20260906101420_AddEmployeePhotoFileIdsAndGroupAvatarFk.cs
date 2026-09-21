using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Community.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeePhotoFileIdsAndGroupAvatarFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CoverPhotoFileId",
                table: "Employees",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PhotoFileId",
                table: "Employees",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1L,
                columns: new[] { "CoverPhotoFileId", "PhotoFileId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2L,
                columns: new[] { "CoverPhotoFileId", "PhotoFileId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3L,
                columns: new[] { "CoverPhotoFileId", "PhotoFileId" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CoverPhotoFileId",
                table: "Employees",
                column: "CoverPhotoFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PhotoFileId",
                table: "Employees",
                column: "PhotoFileId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatConversations_GroupAvatarFileId",
                table: "ChatConversations",
                column: "GroupAvatarFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatConversations_FileStorages_GroupAvatarFileId",
                table: "ChatConversations",
                column: "GroupAvatarFileId",
                principalTable: "FileStorages",
                principalColumn: "FileId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_FileStorages_CoverPhotoFileId",
                table: "Employees",
                column: "CoverPhotoFileId",
                principalTable: "FileStorages",
                principalColumn: "FileId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_FileStorages_PhotoFileId",
                table: "Employees",
                column: "PhotoFileId",
                principalTable: "FileStorages",
                principalColumn: "FileId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatConversations_FileStorages_GroupAvatarFileId",
                table: "ChatConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_FileStorages_CoverPhotoFileId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_FileStorages_PhotoFileId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CoverPhotoFileId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PhotoFileId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_ChatConversations_GroupAvatarFileId",
                table: "ChatConversations");

            migrationBuilder.DropColumn(
                name: "CoverPhotoFileId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PhotoFileId",
                table: "Employees");
        }
    }
}
