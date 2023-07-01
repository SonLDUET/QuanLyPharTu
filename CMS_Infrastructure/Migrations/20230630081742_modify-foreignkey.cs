using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS_Infrastructure.Migrations
{
    public partial class modifyforeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhatTuDaoTrang_PhatTu_PhatTuId",
                table: "PhatTuDaoTrang");

            migrationBuilder.DropForeignKey(
                name: "FK_PhatTuDaoTrang_PhatTu_pt-dt",
                table: "PhatTuDaoTrang");

            migrationBuilder.DropIndex(
                name: "IX_PhatTuDaoTrang_pt-dt",
                table: "PhatTuDaoTrang");

            migrationBuilder.DropColumn(
                name: "pt-dt",
                table: "PhatTuDaoTrang");

            migrationBuilder.AddForeignKey(
                name: "FK_PhatTuDaoTrang_PhatTu_PhatTuId",
                table: "PhatTuDaoTrang",
                column: "PhatTuId",
                principalTable: "PhatTu",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhatTuDaoTrang_PhatTu_PhatTuId",
                table: "PhatTuDaoTrang");

            migrationBuilder.AddColumn<int>(
                name: "pt-dt",
                table: "PhatTuDaoTrang",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PhatTuDaoTrang_pt-dt",
                table: "PhatTuDaoTrang",
                column: "pt-dt");

            migrationBuilder.AddForeignKey(
                name: "FK_PhatTuDaoTrang_PhatTu_PhatTuId",
                table: "PhatTuDaoTrang",
                column: "PhatTuId",
                principalTable: "PhatTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhatTuDaoTrang_PhatTu_pt-dt",
                table: "PhatTuDaoTrang",
                column: "pt-dt",
                principalTable: "PhatTu",
                principalColumn: "Id");
        }
    }
}
