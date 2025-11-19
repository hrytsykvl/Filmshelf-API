using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmShelf.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ModifyPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JsonRespose",
                table: "Pages",
                newName: "MoviesJson");

            migrationBuilder.AlterColumn<int>(
                name: "PageNumber",
                table: "Pages",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PageNumber",
                table: "Pages",
                column: "PageNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pages_PageNumber",
                table: "Pages");

            migrationBuilder.RenameColumn(
                name: "MoviesJson",
                table: "Pages",
                newName: "JsonRespose");

            migrationBuilder.AlterColumn<short>(
                name: "PageNumber",
                table: "Pages",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
