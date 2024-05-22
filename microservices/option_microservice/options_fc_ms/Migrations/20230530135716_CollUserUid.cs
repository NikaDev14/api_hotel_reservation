using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace options_fc_ms.Migrations
{
    /// <inheritdoc />
    public partial class CollUserUid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserUid",
                table: "Options",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUid",
                table: "HotelOptions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserUid",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "UserUid",
                table: "HotelOptions");
        }
    }
}
