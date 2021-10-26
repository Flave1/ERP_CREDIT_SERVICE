using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class cityname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "credit_loancustomer",
                maxLength: 550,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "credit_loancustomer");
        }
    }
}
