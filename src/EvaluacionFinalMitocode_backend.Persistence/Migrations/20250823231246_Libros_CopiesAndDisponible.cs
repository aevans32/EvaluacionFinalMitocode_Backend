using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvaluacionFinalMitocode_backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Libros_CopiesAndDisponible : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Disponible",
                schema: "Productos",
                table: "Libros",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Libros_ISBN",
                schema: "Productos",
                table: "Libros",
                column: "ISBN");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Libros_Titulo",
                schema: "Productos",
                table: "Libros",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Libros_Titulo_Disponible",
                schema: "Productos",
                table: "Libros",
                columns: new[] { "Titulo", "Disponible" },
                filter: "[ActiveStatus] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Productos_Libros_ISBN",
                schema: "Productos",
                table: "Libros");

            migrationBuilder.DropIndex(
                name: "IX_Productos_Libros_Titulo",
                schema: "Productos",
                table: "Libros");

            migrationBuilder.DropIndex(
                name: "IX_Productos_Libros_Titulo_Disponible",
                schema: "Productos",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "Disponible",
                schema: "Productos",
                table: "Libros");
        }
    }
}
