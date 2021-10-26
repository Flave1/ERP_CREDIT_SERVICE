using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class schedulerInvestorfund : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUploaded",
                table: "inf_investorfund",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffEmail",
                table: "inf_investorfund",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUploaded",
                table: "inf_investorfund");

            migrationBuilder.DropColumn(
                name: "StaffEmail",
                table: "inf_investorfund");
        }
    }
}
