using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace csharp_demo_app.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImagesData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    //.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RestaurantId = table.Column<Guid>(nullable: false),
                    ContentType = table.Column<string>(nullable: false),
                    ContentData = table.Column<byte[]>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ImagesData");
        }
    }
}