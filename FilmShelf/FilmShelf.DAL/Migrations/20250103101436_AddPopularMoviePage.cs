using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmShelf.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPopularMoviePage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MoviePages_PageNumber",
                table: "MoviePages");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "MoviePages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Regular");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "MoviePages");

            migrationBuilder.CreateIndex(
                name: "IX_MoviePages_PageNumber",
                table: "MoviePages",
                column: "PageNumber",
                unique: true);
        }
    }
}
