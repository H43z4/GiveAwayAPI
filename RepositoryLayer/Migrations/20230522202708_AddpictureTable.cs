using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class AddpictureTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostImagesId",
                schema: "Setup",
                table: "Post");

            migrationBuilder.CreateTable(
                name: "Pictures",
                schema: "Setup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pictures",
                schema: "Setup");

            migrationBuilder.AddColumn<int>(
                name: "PostImagesId",
                schema: "Setup",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
