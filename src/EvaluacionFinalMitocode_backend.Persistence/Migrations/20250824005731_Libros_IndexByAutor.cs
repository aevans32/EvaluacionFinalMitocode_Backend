using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvaluacionFinalMitocode_backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Libros_IndexByAutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Productos_Libros_Autor",
                schema: "Productos",
                table: "Libros",
                column: "Autor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Productos_Libros_Autor",
                schema: "Productos",
                table: "Libros");
        }
    }
}
