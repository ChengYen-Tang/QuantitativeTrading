using Microsoft.EntityFrameworkCore.Migrations;

namespace MultilateralArbitrage.Migrations
{
    public partial class _2021125 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetsRecords",
                table: "AssetsRecords",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetsRecords",
                table: "AssetsRecords");
        }
    }
}
