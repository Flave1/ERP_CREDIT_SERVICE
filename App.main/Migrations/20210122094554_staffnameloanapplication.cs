using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class staffnameloanapplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StaffName",
                table: "credit_loanapplicationrecommendationlog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffName",
                table: "credit_loanapplicationrecommendationlog");
        }
    }
}
