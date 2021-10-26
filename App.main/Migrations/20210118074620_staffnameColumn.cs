using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class staffnameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StaffName",
                table: "credit_loanreviewapplicationlog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffName",
                table: "credit_loanreviewapplicationlog");
        }
    }
}
