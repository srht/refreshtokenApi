using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace refreshtokenApi.Migrations
{
    public partial class accesstoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "TokenDataId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TokenModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessTokenExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshTokenExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TokenDataId",
                table: "AspNetUsers",
                column: "TokenDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_TokenModel_TokenDataId",
                table: "AspNetUsers",
                column: "TokenDataId",
                principalTable: "TokenModel",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_TokenModel_TokenDataId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TokenModel");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TokenDataId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TokenDataId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
