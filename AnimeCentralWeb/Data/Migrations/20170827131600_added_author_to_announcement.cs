using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnimeCentralWeb.Data.Migrations
{
    public partial class added_author_to_announcement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Announcements",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_AuthorId",
                table: "Announcements",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_AuthorId",
                table: "Announcements",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_AuthorId",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_AuthorId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Announcements");
        }
    }
}
