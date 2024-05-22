using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotel_fc_ms.Migrations
{
    /// <inheritdoc />
    public partial class roomHotelNbItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "nbItems",
                table: "RoomHotels",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nbItems",
                table: "RoomHotels");
        }
    }
}
