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
                   .HasDefaultValue(true);

            // Estado de alquilado o disponible
            builder.Property(x => x.Disponible)
                   .HasDefaultValue(true);                   

            // Index de busquedas por titulo/autor
            builder.HasIndex(x => new { x.Titulo, x.Autor }).HasDatabaseName("IX_Libro_Titulo_Autor");

            builder.HasIndex(x => x.ISBN)
                    .HasDatabaseName("IX_Productos_Libros_ISBN");

            // Fast “all copies of a title” lookups:
            builder.HasIndex(x => x.Titulo)
                   .HasDatabaseName("IX_Productos_Libros_Titulo");

            builder.HasIndex(x => new { x.Titulo, x.Autor })
                   .HasDatabaseName("IX_Productos_Libros_Titulo_Autor");

            // (Optional) find “available copies” fast:
            builder.HasIndex(x => new { x.Titulo, x.Disponible })
                   .HasDatabaseName("IX_Productos_Libros_Titulo_Disponible")
                   .HasFilter("[ActiveStatus] = 1"); // SQL Server filtered index

        }
    }
}
