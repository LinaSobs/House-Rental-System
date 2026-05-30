using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseRentalSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeCheckoutFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountDue",
                table: "RentalApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentCompleted",
                table: "RentalApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "RentalApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDueDate",
                table: "RentalApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentRequired",
                table: "RentalApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "RentalApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountDue",
                table: "RentalApplications");

            migrationBuilder.DropColumn(
                name: "PaymentCompleted",
                table: "RentalApplications");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "RentalApplications");

            migrationBuilder.DropColumn(
                name: "PaymentDueDate",
                table: "RentalApplications");

            migrationBuilder.DropColumn(
                name: "PaymentRequired",
                table: "RentalApplications");

            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "RentalApplications");
        }
    }
}
