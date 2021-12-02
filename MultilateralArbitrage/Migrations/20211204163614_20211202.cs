using Microsoft.EntityFrameworkCore.Migrations;

namespace MultilateralArbitrage.Migrations
{
    public partial class _20211202 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetsRecords",
                table: "AssetsRecords");

            migrationBuilder.RenameTable(
                name: "AssetsRecords",
                newName: "CollisionAssetsRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CollisionAssetsRecords",
                table: "CollisionAssetsRecords",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CollisionAssetsRecords",
                table: "CollisionAssetsRecords");

            migrationBuilder.RenameTable(
                name: "CollisionAssetsRecords",
                newName: "AssetsRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetsRecords",
                table: "AssetsRecords",
                column: "Id");
        }
    }
}
