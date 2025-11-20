using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flow_App.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderFieldsToTaskItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReminderEnabled",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReminderMinutesBefore",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderEnabled",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ReminderMinutesBefore",
                table: "Tasks");
        }
    }
}
