using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnimeCentralWeb.Data.Migrations
{
    public partial class add_anime_to_episode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Anime_AnimeId",
                table: "Episodes");

            migrationBuilder.AlterColumn<int>(
                name: "AnimeId",
                table: "Episodes",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Anime_AnimeId",
                table: "Episodes",
                column: "AnimeId",
                principalTable: "Anime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Anime_AnimeId",
                table: "Episodes");

            migrationBuilder.AlterColumn<int>(
                name: "AnimeId",
                table: "Episodes",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Anime_AnimeId",
                table: "Episodes",
                column: "AnimeId",
                principalTable: "Anime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
