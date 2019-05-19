using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HappeningsDotNetC.Database.Migrations
{
    public partial class SqliteReinstance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    StartRemindAt = table.Column<DateTime>(nullable: false),
                    HappeningTime = table.Column<DateTime>(nullable: false),
                    IsSilenced = table.Column<bool>(nullable: false),
                    HappeningUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    FriendlyName = table.Column<string>(nullable: true),
                    CalendarVisibleToOthers = table.Column<bool>(nullable: false),
                    Hash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Happenings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ControllingUserId = table.Column<Guid>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Happenings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Happenings_Users_ControllingUserId",
                        column: x => x.ControllingUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HappeningUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HappeningId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    UserStatus = table.Column<int>(nullable: false),
                    ReminderXMinsBefore = table.Column<int>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    ReminderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HappeningUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HappeningUser_Happenings_HappeningId",
                        column: x => x.HappeningId,
                        principalTable: "Happenings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HappeningUser_Reminders_ReminderId",
                        column: x => x.ReminderId,
                        principalTable: "Reminders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HappeningUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Happenings_ControllingUserId",
                table: "Happenings",
                column: "ControllingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HappeningUser_HappeningId",
                table: "HappeningUser",
                column: "HappeningId");

            migrationBuilder.CreateIndex(
                name: "IX_HappeningUser_ReminderId",
                table: "HappeningUser",
                column: "ReminderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HappeningUser_UserId",
                table: "HappeningUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HappeningUser");

            migrationBuilder.DropTable(
                name: "Happenings");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
