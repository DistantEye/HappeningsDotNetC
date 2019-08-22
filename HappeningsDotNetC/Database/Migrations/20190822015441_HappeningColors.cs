using Microsoft.EntityFrameworkCore.Migrations;

namespace HappeningsDotNetC.Database.Migrations
{
    public partial class HappeningColors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Flavor",
                table: "Happenings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flavor",
                table: "Happenings");
        }
    }
}
