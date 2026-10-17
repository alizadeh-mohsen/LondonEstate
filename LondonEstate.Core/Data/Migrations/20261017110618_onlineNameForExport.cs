using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonEstate.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class onlineNameForExport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OnlineName",
                table: "FlatBackup",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlineName",
                table: "FlatBackup");
        }
    }
}
