using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Community.Migrations
{
    /// <inheritdoc />
    public partial class AddRecognitionBadgesJoinTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecognitionBadges",
                columns: table => new
                {
                    RecognitionBadgeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecognitionId = table.Column<long>(type: "bigint", nullable: false),
                    BadgeId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecognitionBadges", x => x.RecognitionBadgeId);
                    table.ForeignKey(
                        name: "FK_RecognitionBadges_Badges_BadgeId",
                        column: x => x.BadgeId,
                        principalTable: "Badges",
                        principalColumn: "BadgeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecognitionBadges_Recognitions_RecognitionId",
                        column: x => x.RecognitionId,
                        principalTable: "Recognitions",
                        principalColumn: "RecognitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecognitionBadges_BadgeId",
                table: "RecognitionBadges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecognitionBadges_RecognitionId_BadgeId",
                table: "RecognitionBadges",
                columns: new[] { "RecognitionId", "BadgeId" },
                unique: true);

            // Preserve existing single-badge links by copying them into the new join table
            // before the old column is dropped below.
            migrationBuilder.Sql(@"
                INSERT INTO ""RecognitionBadges"" (""RecognitionId"", ""BadgeId"", ""TenantId"", ""Status"", ""CreatedAt"")
                SELECT ""RecognitionId"", ""BadgeId"", ""TenantId"", 0, COALESCE(""CreatedAt"", now())
                FROM ""Recognitions""
                WHERE ""BadgeId"" IS NOT NULL;
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Recognitions_Badges_BadgeId",
                table: "Recognitions");

            migrationBuilder.DropIndex(
                name: "IX_Recognitions_BadgeId",
                table: "Recognitions");

            migrationBuilder.DropColumn(
                name: "BadgeId",
                table: "Recognitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BadgeId",
                table: "Recognitions",
                type: "bigint",
                nullable: true);

            // Backfill from the join table - a recognition with multiple badges after the
            // upgrade path arbitrarily keeps its lowest BadgeId when downgrading.
            migrationBuilder.Sql(@"
                UPDATE ""Recognitions"" r
                SET ""BadgeId"" = sub.""BadgeId""
                FROM (
                    SELECT DISTINCT ON (""RecognitionId"") ""RecognitionId"", ""BadgeId""
                    FROM ""RecognitionBadges""
                    ORDER BY ""RecognitionId"", ""BadgeId""
                ) sub
                WHERE r.""RecognitionId"" = sub.""RecognitionId"";
            ");

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

            migrationBuilder.DropTable(
                name: "RecognitionBadges");
        }
    }
}
