using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KYCVerificationAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Verifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(25)", nullable: false),
                    GivenName = table.Column<string>(type: "varchar(25)", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Nin = table.Column<string>(type: "varchar(14)", nullable: false),
                    CardNumber = table.Column<string>(type: "varchar(10)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(25)", nullable: false),
                    CorrelationId = table.Column<string>(type: "varchar(50)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    KycStatus = table.Column<int>(type: "integer", nullable: false),
                    Retries = table.Column<int>(type: "integer", nullable: true),
                    NameAsPerIdMatches = table.Column<bool>(type: "boolean", nullable: true),
                    NinAsPerIdMatches = table.Column<bool>(type: "boolean", nullable: true),
                    CardNumberAsPerIdMatches = table.Column<bool>(type: "boolean", nullable: true),
                    DateOfBirthMatches = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_CardNumber",
                table: "Verifications",
                column: "CardNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_CreatedAt",
                table: "Verifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_CreatedBy",
                table: "Verifications",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_FirstName",
                table: "Verifications",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_GivenName",
                table: "Verifications",
                column: "GivenName");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_Nin",
                table: "Verifications",
                column: "Nin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Verifications");
        }
    }
}
