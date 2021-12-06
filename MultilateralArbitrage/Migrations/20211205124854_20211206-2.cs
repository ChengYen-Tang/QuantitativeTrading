using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MultilateralArbitrage.Migrations
{
    public partial class _202112062 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CollisionAssetsRecords");

            migrationBuilder.CreateTable(
                name: "CollisionAndLastStepPaddingAssetsRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarketMix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Assets = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollisionAndLastStepPaddingAssetsRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollisionAndLastStepPaddingAssetsRecords");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CollisionAssetsRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
