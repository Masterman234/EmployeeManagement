using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagement.Migrations
{
    /// <inheritdoc />
    public partial class Userregistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create the new table FIRST
            migrationBuilder.CreateTable(
                name: "EmployeeEducationQualifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeEducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeEducationQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeEducationQualifications_EmployeeEducations_EmployeeEducationId",
                        column: x => x.EmployeeEducationId,
                        principalTable: "EmployeeEducations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
        INSERT INTO EmployeeEducationQualifications (Id, EmployeeEducationId, Qualification, CreatedAt)
        SELECT NEWID(), ee.Id, LTRIM(RTRIM(split.value)), GETUTCDATE()
        FROM EmployeeEducations ee
        CROSS APPLY STRING_SPLIT(ee.Qualifications, ',') AS split
        WHERE ee.Qualifications IS NOT NULL
          AND ee.Qualifications <> ''
          AND LTRIM(RTRIM(split.value)) <> ''
    ");

            migrationBuilder.DropColumn(
                name: "Qualifications",
                table: "EmployeeEducations");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEducationQualifications_EmployeeEducationId",
                table: "EmployeeEducationQualifications",
                column: "EmployeeEducationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeEducationQualifications");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Qualifications",
                table: "EmployeeEducations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
