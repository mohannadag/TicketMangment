using Microsoft.EntityFrameworkCore.Migrations;

namespace TicketMangment.Migrations
{
    public partial class AddingInDepRoleToUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InDepRole",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InDepRole",
                table: "AspNetUsers");
        }
    }
}
