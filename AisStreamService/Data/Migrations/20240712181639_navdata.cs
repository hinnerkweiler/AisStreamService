using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AisStreamService.Data.Migrations
{
    /// <inheritdoc />
    public partial class navdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Course",
                table: "AIS_Vessels",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Speed",
                table: "AIS_Vessels",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Course",
                table: "AIS_Vessels");

            migrationBuilder.DropColumn(
                name: "Speed",
                table: "AIS_Vessels");
        }
    }
}
