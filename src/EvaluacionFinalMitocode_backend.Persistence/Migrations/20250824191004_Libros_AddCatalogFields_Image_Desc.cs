using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvaluacionFinalMitocode_backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Libros_AddCatalogFields_Image_Desc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Productos",
                table: "Libros",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtendedDescription",
                schema: "Productos",
                table: "Libros",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                schema: "Productos",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "Productos",
                table: "Libros",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                schema: "Productos",
                table: "Libros",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Productos_Libros_ISBN_DigitsOnly",
                schema: "Productos",
                table: "Libros",
                sql: "[ISBN] NOT LIKE '%[^0-9]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Productos_Libros_UnitPrice_NonNegative",
                schema: "Productos",
                table: "Libros",
                sql: "[UnitPrice] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Productos_Libros_ISBN_DigitsOnly",
                schema: "Productos",
                table: "Libros");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Productos_Libros_UnitPrice_NonNegative",
                schema: "Productos",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Productos",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "ExtendedDescription",
                schema: "Productos",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "GenreId",
                schema: "Productos",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "Productos",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                schema: "Productos",
                table: "Libros");
        }
    }
}
