using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvaluacionFinalMitocode_backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Users");

            migrationBuilder.CreateSequence<int>(
                name: "ClienteSeq",
                schema: "Users");

            migrationBuilder.CreateTable(
                name: "Clientes",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(7)", unicode: false, maxLength: 7, nullable: false, defaultValueSql: "'CLT' + RIGHT(REPLICATE('0', 4) + CAST(NEXT VALUE FOR [Users].[ClienteSeq] AS VARCHAR(10)), 4)"),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DNI = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActiveStatus = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.CheckConstraint("CK_Users_Clientes_DNI", "LEN([DNI]) = 8 AND [DNI] NOT LIKE '%[^0-9]%'");
                    table.CheckConstraint("CK_Users_Clientes_Edad", "[Edad] BETWEEN 0 AND 120");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Apellidos_Nombres",
                schema: "Users",
                table: "Clientes",
                columns: new[] { "Apellidos", "Nombres" });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_DNI",
                schema: "Users",
                table: "Clientes",
                column: "DNI",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes",
                schema: "Users");

            migrationBuilder.DropSequence(
                name: "ClienteSeq",
                schema: "Users");
        }
    }
}
