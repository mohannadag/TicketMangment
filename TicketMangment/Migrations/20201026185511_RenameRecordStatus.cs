using Microsoft.EntityFrameworkCore.Migrations;

namespace TicketMangment.Migrations
{
    public partial class RenameRecordStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusRecord",
                table: "TicketDetails");

            migrationBuilder.DropColumn(
                name: "StatusRecord",
                table: "Priorities");

            migrationBuilder.DropColumn(
                name: "StatusRecord",
                table: "Departments");

            migrationBuilder.AddColumn<int>(
                name: "RecordStatus",
                table: "TicketDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordStatus",
                table: "Priorities",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordStatus",
                table: "Notes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordStatus",
                table: "Departments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordStatus",
                table: "TicketDetails");

            migrationBuilder.DropColumn(
                name: "RecordStatus",
                table: "Priorities");

            migrationBuilder.DropColumn(
                name: "RecordStatus",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "RecordStatus",
                table: "Departments");

            migrationBuilder.AddColumn<int>(
                name: "StatusRecord",
                table: "TicketDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusRecord",
                table: "Priorities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusRecord",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
