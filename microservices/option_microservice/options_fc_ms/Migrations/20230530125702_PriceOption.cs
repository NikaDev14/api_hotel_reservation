using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace options_fc_ms.Migrations
{
    /// <inheritdoc />
    public partial class PriceOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "Options",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Options");
        }
    }
}
