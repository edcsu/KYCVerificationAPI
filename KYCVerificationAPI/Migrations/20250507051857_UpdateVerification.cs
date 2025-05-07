using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KYCVerificationAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Verifications",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KycMessage",
                table: "Verifications",
                type: "varchar(50)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KycMessage",
                table: "Verifications");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Verifications",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
