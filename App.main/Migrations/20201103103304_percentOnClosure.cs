using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class percentOnClosure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Percentage",
                table: "deposit_bankclosuresetup",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "deposit_bankclosuresetup");
        }
    }
}
