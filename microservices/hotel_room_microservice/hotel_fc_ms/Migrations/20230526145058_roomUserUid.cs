using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotel_fc_ms.Migrations
{
    /// <inheritdoc />
    public partial class roomUserUid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserUid",
                table: "RoomHotels",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserUid",
                table: "RoomHotels");
        }
    }
}
