﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EquipmentManagementSystem.Migrations
{
    public partial class @new : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    LastEdited = table.Column<DateTime>(nullable: false),
                    Model = table.Column<string>(nullable: true),
                    Serial = table.Column<string>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    OwnerID = table.Column<int>(nullable: true),
                    OwnerName = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    EquipType = table.Column<int>(nullable: false),
                    MobileNumber = table.Column<string>(nullable: true),
                    IP = table.Column<string>(nullable: true),
                    Ports = table.Column<int>(nullable: true),
                    Resolution = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    LastEdited = table.Column<DateTime>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    SSN = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    TelNr = table.Column<string>(nullable: true),
                    Mail = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "Owners");
        }
    }
}
