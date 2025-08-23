using EvaluacionFinalMitocode_backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvaluacionFinalMitocode_backend.Persistence.Configurations
{
    public class LibroConfiguration : IEntityTypeConfiguration<Libro>
    {
        public void Configure(EntityTypeBuilder<Libro> builder)
        {
            builder.ToTable("Libros", schema: "Productos");

            // PK: string auto-generada en la BD -> "LBR" + 4 dígitos (LBR0001)
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .IsRequired()
                   .HasMaxLength(7)      // "LBR0001"
                   .IsUnicode(false)
                   .ValueGeneratedOnAdd()
                   .HasDefaultValueSql(
                        "'LBR' + RIGHT(REPLICATE('0', 4) + CAST(NEXT VALUE FOR [Productos].[LibroSeq] AS VARCHAR(10)), 4)"
                    );

            // Campos requeridos
            builder.Property(x => x.Titulo).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Autor).IsRequired().HasMaxLength(100);

            // ISBN: 13 caracteres, solo numeros y guiones, unico, no-unicode
            builder.Property(x => x.ISBN)
                   .IsRequired()
                   .HasMaxLength(13)
                   .IsUnicode(false);

            // Estado disponible y soft-delete (Active Status)
            builder.Property(x => x.ActiveStatus)
                   .HasDefaultValue(true)
                   .ValueGeneratedOnAdd();

            // Index de busquedas por titulo/autor
            builder.HasIndex(x => new { x.Titulo, x.Autor }).HasDatabaseName("IX_Libro_Titulo_Autor");

        }
    }
}
