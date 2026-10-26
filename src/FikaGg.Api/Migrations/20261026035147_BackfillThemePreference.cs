using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FikaGg.Api.Migrations
{
    /// <inheritdoc />
    public partial class BackfillThemePreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("""
				UPDATE "AspNetUsers"
				SET "ThemePreference" = 'dark'
				WHERE "ThemePreference" = '';
				""");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
