using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class phoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingNumber",
                table: "FlatBackup",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuestPhone",
                table: "FlatBackup",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuestPhone",
                table: "Flat",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingNumber",
                table: "FlatBackup");

            migrationBuilder.DropColumn(
                name: "GuestPhone",
                table: "FlatBackup");

            migrationBuilder.DropColumn(
                name: "GuestPhone",
                table: "Flat");
        }
    }
}
