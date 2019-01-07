using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HappeningsDotNetC.Database.Migrations
{
    public partial class ReminderRevisions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_HappeningUser_HappeningUserId",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_HappeningUserId",
                table: "Reminders");

            migrationBuilder.AddColumn<Guid>(
                name: "ReminderId",
                table: "HappeningUser",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_HappeningUser_ReminderId",
                table: "HappeningUser",
                column: "ReminderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HappeningUser_Reminders_ReminderId",
                table: "HappeningUser",
                column: "ReminderId",
                principalTable: "Reminders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HappeningUser_Reminders_ReminderId",
                table: "HappeningUser");

            migrationBuilder.DropIndex(
                name: "IX_HappeningUser_ReminderId",
                table: "HappeningUser");

            migrationBuilder.DropColumn(
                name: "ReminderId",
                table: "HappeningUser");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_HappeningUserId",
                table: "Reminders",
                column: "HappeningUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_HappeningUser_HappeningUserId",
                table: "Reminders",
                column: "HappeningUserId",
                principalTable: "HappeningUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
