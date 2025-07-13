using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueLagoon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingVillaSuite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ViilaNumber",
                table: "Bookings",
                newName: "VillaSuite");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VillaSuite",
                table: "Bookings",
                newName: "ViilaNumber");
        }
    }
}
