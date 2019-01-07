using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HappeningsDotNetC.Database.Migrations
{
    public partial class RoughDraftDBLayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventUser");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Users",
                nullable: true);

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
                    IsPrivate = table.Column<bool>(nullable: false)
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
                        name: "FK_HappeningUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    table.ForeignKey(
                        name: "FK_Reminders_HappeningUser_HappeningUserId",
                        column: x => x.HappeningUserId,
                        principalTable: "HappeningUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_HappeningUser_UserId",
                table: "HappeningUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_HappeningUserId",
                table: "Reminders",
                column: "HappeningUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "HappeningUser");

            migrationBuilder.DropTable(
                name: "Happenings");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FriendlyName = table.Column<DateTime>(nullable: false),
                    PrimaryUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Users_PrimaryUserId",
                        column: x => x.PrimaryUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EventId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventUser_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_PrimaryUserId",
                table: "Event",
                column: "PrimaryUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_EventId",
                table: "EventUser",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_UserId",
                table: "EventUser",
                column: "UserId");
        }
    }
}
