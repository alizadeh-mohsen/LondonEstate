using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonEstate.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class listing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlineName",
                table: "FlatBackup");

            migrationBuilder.DropColumn(
                  name: "FlatUrl",
                  table: "Flat");

            migrationBuilder.DropColumn(
                name: "ReservationUrl",
                table: "Flat");


            migrationBuilder.DropColumn(
                name: "VisualGuideUrl",
                table: "Flat");

            migrationBuilder.CreateTable(
                name: "Listing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnlineName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listing_Flat_FlatId",
                        column: x => x.FlatId,
                        principalTable: "Flat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Listing_FlatId",
                table: "Listing",
                column: "FlatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Listing");

            migrationBuilder.AddColumn<string>(
                name: "OnlineName",
                table: "FlatBackup",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlatUrl",
                table: "Flat",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReservationUrl",
                table: "Flat",
                type: "nvarchar(max)",
                nullable: true);


            migrationBuilder.AddColumn<string>(
                name: "VisualGuideUrl",
                table: "Flat",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
