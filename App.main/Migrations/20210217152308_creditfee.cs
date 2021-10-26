using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class creditfee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PassEntryAtDisbursment",
                table: "credit_fee",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassEntryAtDisbursment",
                table: "credit_fee");
        }
    }
}
