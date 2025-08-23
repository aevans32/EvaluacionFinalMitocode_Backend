using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvaluacionFinalMitocode_backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLibros_ProductosSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Productos");

            migrationBuilder.CreateSequence<int>(
                name: "LibroSeq",
                schema: "Productos");

            migrationBuilder.CreateTable(
                name: "Libros",
                schema: "Productos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(7)", unicode: false, maxLength: 7, nullable: false, defaultValueSql: "'LBR' + RIGHT(REPLICATE('0', 4) + CAST(NEXT VALUE FOR [Productos].[LibroSeq] AS VARCHAR(10)), 4)"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "varchar(13)", unicode: false, maxLength: 13, nullable: false),
                    ActiveStatus = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Libro_Titulo_Autor",
                schema: "Productos",
                table: "Libros",
                columns: new[] { "Titulo", "Autor" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Libros",
                schema: "Productos");

            migrationBuilder.DropSequence(
                name: "LibroSeq",
                schema: "Productos");
        }
    }
}
