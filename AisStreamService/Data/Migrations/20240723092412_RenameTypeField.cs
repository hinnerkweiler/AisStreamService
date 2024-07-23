using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AisStreamService.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTypeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "AIS_Vessels",
                newName: "ShipType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShipType",
                table: "AIS_Vessels",
                newName: "Type");
        }
    }
}
