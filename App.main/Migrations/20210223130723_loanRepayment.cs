﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class loanRepayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "deposit_depositform",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "deposit_depositform");
        }
    }
}
