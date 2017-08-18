using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnimeCentralWeb.Data.Migrations
{
    public partial class add_translate_status_changed_noofepisodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NoOfEpisodes",
                table: "Anime",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TranslateStatus",
                table: "Anime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranslateStatus",
                table: "Anime");

            migrationBuilder.AlterColumn<string>(
                name: "NoOfEpisodes",
                table: "Anime",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
