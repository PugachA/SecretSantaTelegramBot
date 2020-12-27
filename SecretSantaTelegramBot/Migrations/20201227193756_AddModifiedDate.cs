using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSantaTelegramBot.Migrations
{
    public partial class AddModifiedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Users",
                newName: "ModifiedDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Users",
                newName: "Date");
        }
    }
}
