using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnimeCentralWeb.Data.Migrations
{
    public partial class add_source_origin_and_content_to_source : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "Sources",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Origin",
                table: "Sources",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Sources");
        }
    }
}
