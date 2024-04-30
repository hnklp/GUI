using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.Migrations
{
    /// <inheritdoc />
    public partial class date : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mobs_Species_SpeciesId",
                table: "Mobs");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Mobs",
                newName: "DateOfCapture");

            migrationBuilder.AlterColumn<int>(
                name: "SpeciesId",
                table: "Mobs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Mobs_Species_SpeciesId",
                table: "Mobs",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "SpeciesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mobs_Species_SpeciesId",
                table: "Mobs");

            migrationBuilder.RenameColumn(
                name: "DateOfCapture",
                table: "Mobs",
                newName: "Date");

            migrationBuilder.AlterColumn<int>(
                name: "SpeciesId",
                table: "Mobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Mobs_Species_SpeciesId",
                table: "Mobs",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "SpeciesId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
