using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnimeCentralWeb.Data.Migrations
{
    public partial class changed_content_to_filename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Sources");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Sources",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Sources");

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "Sources",
                nullable: true);
        }
    }
}
