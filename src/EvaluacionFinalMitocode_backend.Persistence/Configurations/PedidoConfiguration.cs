using EvaluacionFinalMitocode_backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvaluacionFinalMitocode_backend.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos", "Ventas");

        // PK "PED0001" (7 chars)
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
               .IsRequired()
               .HasMaxLength(7)
               .IsUnicode(false)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql(
                   "'PED' + RIGHT(REPLICATE('0', 4) + CAST(NEXT VALUE FOR [Ventas].[PedidoSeq] AS VARCHAR(10)), 4)"
               );

        builder.Property(p => p.ClienteId)
                   .IsRequired()
                   .HasMaxLength(7); // "CLT0001" (ajusta si usas otro largo)

        builder.Property(p => p.FechaPedido)
               .IsRequired()
               .HasColumnType("datetime2")
               .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(p => p.FechaEntrega)
               .HasColumnType("datetime2");

        builder.Property(p => p.MontoTotal)
               .HasPrecision(18, 2)
               .HasDefaultValue(0m);

        builder.Property(p => p.Estado)
               .IsRequired()
               .HasDefaultValue(true);

        // FK -> Cliente
        builder.HasOne(p => p.Cliente)
               .WithMany() // si luego agregas ICollection<Pedido> en Cliente, cámbialo a .WithMany(c => c.Pedidos)
               .HasForeignKey(p => p.ClienteId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_Pedidos_Cliente");

        // Índices útiles para reporting / filtros
        builder.HasIndex(p => p.ClienteId).HasDatabaseName("IX_Pedidos_ClienteId");
        builder.HasIndex(p => p.FechaPedido).HasDatabaseName("IX_Pedidos_FechaPedido");
        builder.HasIndex(p => p.Estado).HasDatabaseName("IX_Pedidos_Estado");

        // Check constraints
        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("CK_Ventas_Pedidos_MontoTotal", "[MontoTotal] >= 0");
            tb.HasCheckConstraint("CK_Ventas_Pedidos_Fechas", "[FechaEntrega] IS NULL OR [FechaEntrega] >= [FechaPedido]");
        });
    }
}

public class PedidoLibroConfiguration : IEntityTypeConfiguration<PedidoLibro>
{
    public void Configure(EntityTypeBuilder<PedidoLibro> builder)
    {
        builder.ToTable("PedidoLibros", "Ventas");

        // PK compuesta evita duplicar el mismo libro en el mismo pedido
        builder.HasKey(pl => new { pl.PedidoId, pl.LibroId });

        builder.Property(pl => pl.PedidoId)
               .IsRequired()
               .HasMaxLength(7)
               .IsUnicode(false);

        builder.Property(pl => pl.LibroId)
               .IsRequired()
               .HasMaxLength(7)
               .IsUnicode(false);

        builder.Property(pl => pl.Cantidad)
               .IsRequired();

        builder.Property(pl => pl.PrecioUnitario)
               .HasPrecision(18, 2)
               .IsRequired();

        // FKs
        builder.HasOne(pl => pl.Pedido)
               .WithMany(p => p.PedidoLibros)
               .HasForeignKey(pl => pl.PedidoId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_PedidoLibros_Pedido");

        builder.HasOne(pl => pl.Libro)
               .WithMany()
               .HasForeignKey(pl => pl.LibroId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_PedidoLibros_Libro");

        // Índices
        builder.HasIndex(pl => pl.LibroId).HasDatabaseName("IX_PedidoLibros_LibroId");

        // Checks
        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("CK_Ventas_PedidoLibros_Cantidad", "[Cantidad] > 0");
            tb.HasCheckConstraint("CK_Ventas_PedidoLibros_Precio", "[PrecioUnitario] >= 0");
        });
    }
}
