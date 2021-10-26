using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class loancheque : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "credit_loan_cheque_list",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "credit_loan_cheque_list");
        }
    }
}
