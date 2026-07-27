using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class backupfee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPayment",
                table: "FlatBackup",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPayment",
                table: "FlatBackup");
        }
    }
}
