using Microsoft.EntityFrameworkCore.Migrations;

namespace MultilateralArbitrage.Migrations
{
    public partial class _202112061 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetsRecord",
                table: "AssetsRecord");

            migrationBuilder.RenameTable(
                name: "AssetsRecord",
                newName: "CollisionAssetsRecords");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CollisionAssetsRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CollisionAssetsRecords");

            migrationBuilder.RenameTable(
                name: "CollisionAssetsRecords",
                newName: "AssetsRecord");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetsRecord",
                table: "AssetsRecord",
                column: "Id");
        }
    }
}
