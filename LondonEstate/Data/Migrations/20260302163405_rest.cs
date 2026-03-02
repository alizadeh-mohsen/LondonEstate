using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class rest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RentHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RentHistory_RentId",
                table: "RentHistory",
                column: "RentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rent");

            migrationBuilder.DropTable(
                name: "RentHistory");
        }
    }
}
