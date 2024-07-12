using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AisStreamService.Data.Migrations
{
    /// <inheritdoc />
    public partial class addmetadataforships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "AIS_Vessels",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "AIS_Vessels",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AIS_Vessels",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ShipUrl",
                table: "AIS_Vessels",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AIS_Vessels",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "AIS_Vessels");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "AIS_Vessels");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AIS_Vessels");

            migrationBuilder.DropColumn(
                name: "ShipUrl",
                table: "AIS_Vessels");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AIS_Vessels");
        }
    }
}
