﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BadBrokerTestTask.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "CurrencyRateModels",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CurrencyRateModels", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyRateModels");
        }
    }
}
