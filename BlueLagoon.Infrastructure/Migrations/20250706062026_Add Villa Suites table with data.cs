using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlueLagoon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVillaSuitestablewithdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VillaSuites",
                columns: table => new
                {
                    VillaSuitId = table.Column<int>(type: "int", nullable: false),
                    VillaId = table.Column<int>(type: "int", nullable: false),
                    SpecialDetails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VillaSuites", x => x.VillaSuitId);
                    table.ForeignKey(
                        name: "FK_VillaSuites_Villas_VillaId",
                        column: x => x.VillaId,
                        principalTable: "Villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "VillaSuites",
                columns: new[] { "VillaSuitId", "SpecialDetails", "VillaId" },
                values: new object[,]
                {
                    { 101, null, 1 },
                    { 102, null, 1 },
                    { 103, null, 1 },
                    { 201, null, 2 },
                    { 202, null, 2 },
                    { 203, null, 2 },
                    { 301, null, 3 },
                    { 302, null, 3 },
                    { 303, null, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_VillaSuites_VillaId",
                table: "VillaSuites",
                column: "VillaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VillaSuites");
        }
    }
}
