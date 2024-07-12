using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AisStreamService.Data.Migrations
{
    /// <inheritdoc />
    public partial class renamegroupfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Group",
                table: "AIS_Vessels",
                newName: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "AIS_Vessels",
                newName: "Group");
        }
    }
}
