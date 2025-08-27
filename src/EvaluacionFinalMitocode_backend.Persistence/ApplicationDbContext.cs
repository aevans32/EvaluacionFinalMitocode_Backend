using EvaluacionFinalMitocode_backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EvaluacionFinalMitocode_backend.Persistence;

// TODO: Usar esta clase para manejar la autenticacion y autorizacion de usuarios
//public class ApplicationDbContext : IdentityDbContext<EFUserIdentity>
// public class ApplicationDbContext : DbContext
public class ApplicationDbContext : IdentityDbContext<EFUserIdentity>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Libro> Libros => Set<Libro>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoLibro> PedidoLibros => Set<PedidoLibro>();

    // Personalizar Entidades
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("dbo");
        modelBuilder.HasSequence<int>("ClienteSeq", schema: "Users").StartsAt(1).IncrementsBy(1);
        modelBuilder.HasSequence<int>("LibroSeq", schema: "Productos").StartsAt(1).IncrementsBy(1);
        modelBuilder.HasSequence<int>("PedidoSeq", schema: "Ventas").StartsAt(1).IncrementsBy(1);


        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<EFUserIdentity>(x => x.ToTable("User"));
        modelBuilder.Entity<IdentityRole>(x => x.ToTable("Role"));
        modelBuilder.Entity<IdentityUserRole<string>>(x => x.ToTable("UserRole", "Auth"));
        modelBuilder.Entity<IdentityUserLogin<string>>(x => x.ToTable("UserLogin", "Auth"));
        modelBuilder.Entity<IdentityUserToken<string>>(x => x.ToTable("UserToken", "Auth"));
        modelBuilder.Entity<IdentityUserClaim<string>>(x => x.ToTable("UserClaim", "Auth"));
        modelBuilder.Entity<IdentityRoleClaim<string>>(x => x.ToTable("RoleClaim", "Auth"));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseLazyLoadingProxies();
    }
}
