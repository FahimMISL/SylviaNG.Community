using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Community.Migrations
{
    /// <inheritdoc />
    public partial class AddChatMessagePinFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "ChatMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PinnedAt",
                table: "ChatMessages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PinnedByEmployeeId",
                table: "ChatMessages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatConversationId_IsPinned",
                table: "ChatMessages",
                columns: new[] { "ChatConversationId", "IsPinned" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ChatConversationId_IsPinned",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "PinnedAt",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "PinnedByEmployeeId",
                table: "ChatMessages");
        }
    }
}
