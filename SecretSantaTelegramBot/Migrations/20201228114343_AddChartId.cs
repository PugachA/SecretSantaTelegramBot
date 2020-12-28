using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSantaTelegramBot.Migrations
{
    public partial class AddChartId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChartId",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChartId",
                table: "Users");
        }
    }
}
