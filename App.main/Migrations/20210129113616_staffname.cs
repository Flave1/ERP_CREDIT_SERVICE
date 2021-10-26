using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class staffname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUploaded",
                table: "credit_loan",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "staffEmail",
                table: "credit_loan",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUploaded",
                table: "credit_loan");

            migrationBuilder.DropColumn(
                name: "staffEmail",
                table: "credit_loan");
        }
    }
}
