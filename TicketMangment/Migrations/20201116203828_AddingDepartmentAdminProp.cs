using Microsoft.EntityFrameworkCore.Migrations;

namespace TicketMangment.Migrations
{
    public partial class AddingDepartmentAdminProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InDepRole",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "DepAdmin",
                table: "Departments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepAdmin",
                table: "Departments");

            migrationBuilder.AddColumn<int>(
                name: "InDepRole",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
