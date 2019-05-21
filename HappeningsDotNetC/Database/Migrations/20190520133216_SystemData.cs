using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HappeningsDotNetC.Database.Migrations
{
    public partial class SystemData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "_SystemData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OpenRegistration = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SystemData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_SystemData");
        }
    }
}
