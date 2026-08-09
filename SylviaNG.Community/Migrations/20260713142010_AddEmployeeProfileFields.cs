using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Community.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Achievements",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommunityContributions",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Employees",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employees",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailVisibility",
                table: "Employees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Employees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtensionVisibility",
                table: "Employees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "GradeId",
                table: "Employees",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Interests",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Employees",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneVisibility",
                table: "Employees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Employees",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "Achievements", "Bio", "CommunityContributions", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DepartmentId", "DesignatioId", "Division", "Email", "EmailVisibility", "EmployeeCode", "EmployeeName", "Extension", "ExtensionVisibility", "GradeId", "Interests", "IsActive", "Phone", "PhoneVisibility", "PhotoUrl", "RFId", "Remarks", "SiteId", "Skills", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, null, "Frontend engineer who enjoys mentoring new hires.", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, 1L, 1L, "Engineering", "ayesha.rahman@sylviang.example", "Private", "EMP00001", "Ayesha Rahman", "1001", "Private", null, "Photography, Chess", true, "+880-1710-000001", "Private", null, null, null, 1L, "Angular, TypeScript, PrimeNG", 1, "default_tenant", null, null },
                    { 2L, null, "Engineering team lead focused on delivery quality.", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, 1L, 2L, "Engineering", "tanvir.hasan@sylviang.example", "Public", "EMP00002", "Tanvir Hasan", "1002", "Private", null, "Cricket, Reading", true, "+880-1710-000002", "Public", null, null, null, 1L, "Leadership, .NET, SQL", 1, "default_tenant", null, null },
                    { 3L, null, "HR Business Partner supporting the engineering org.", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, 2L, 3L, "People & Culture", "farhana.akter@sylviang.example", "Public", "EMP00003", "Farhana Akter", "1003", "Private", null, "Yoga, Travel", true, "+880-1710-000003", "Private", null, null, null, 1L, "Recruiting, Employee Relations", 1, "default_tenant", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IsActive",
                table: "Employees",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SiteId",
                table: "Employees",
                column: "SiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_IsActive",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_SiteId",
                table: "Employees");

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3L);

            migrationBuilder.DropColumn(
                name: "Achievements",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CommunityContributions",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmailVisibility",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ExtensionVisibility",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Interests",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PhoneVisibility",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Employees");
        }
    }
}
