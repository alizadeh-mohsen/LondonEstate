using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonEstate.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class ListingName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OnlineName",
                table: "Listing",
                newName: "ListingName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ListingName",
                table: "Listing",
                newName: "OnlineName");
        }
    }
}
