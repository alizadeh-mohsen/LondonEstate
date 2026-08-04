using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class removedpRICE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPayment",
                table: "FlatBackup");

            migrationBuilder.DropColumn(
                name: "TotalPayment",
                table: "Flat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPayment",
                table: "FlatBackup",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPayment",
                table: "Flat",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
