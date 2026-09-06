using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Community.Migrations
{
    /// <inheritdoc />
    public partial class AddFileStorageIdToAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FileStorageId",
                table: "TaskAttachments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileStorageId",
                table: "PostAttachments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileStorageId",
                table: "ListingImages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_FileStorageId",
                table: "TaskAttachments",
                column: "FileStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_PostAttachments_FileStorageId",
                table: "PostAttachments",
                column: "FileStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingImages_FileStorageId",
                table: "ListingImages",
                column: "FileStorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingImages_FileStorages_FileStorageId",
                table: "ListingImages",
                column: "FileStorageId",
                principalTable: "FileStorages",
                principalColumn: "FileId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostAttachments_FileStorages_FileStorageId",
                table: "PostAttachments",
                column: "FileStorageId",
                principalTable: "FileStorages",
                principalColumn: "FileId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAttachments_FileStorages_FileStorageId",
                table: "TaskAttachments",
                column: "FileStorageId",
                principalTable: "FileStorages",
                principalColumn: "FileId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListingImages_FileStorages_FileStorageId",
                table: "ListingImages");

            migrationBuilder.DropForeignKey(
                name: "FK_PostAttachments_FileStorages_FileStorageId",
                table: "PostAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskAttachments_FileStorages_FileStorageId",
                table: "TaskAttachments");

            migrationBuilder.DropIndex(
                name: "IX_TaskAttachments_FileStorageId",
                table: "TaskAttachments");

            migrationBuilder.DropIndex(
                name: "IX_PostAttachments_FileStorageId",
                table: "PostAttachments");

            migrationBuilder.DropIndex(
                name: "IX_ListingImages_FileStorageId",
                table: "ListingImages");

            migrationBuilder.DropColumn(
                name: "FileStorageId",
                table: "TaskAttachments");

            migrationBuilder.DropColumn(
                name: "FileStorageId",
                table: "PostAttachments");

            migrationBuilder.DropColumn(
                name: "FileStorageId",
                table: "ListingImages");
        }
    }
}
