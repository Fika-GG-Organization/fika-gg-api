using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FikaGg.Api.Migrations
{
    /// <inheritdoc />
    public partial class CloudinarySetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarPublicId",
                table: "AspNetUsers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarPublicId",
                table: "AspNetUsers");
        }
    }
}
