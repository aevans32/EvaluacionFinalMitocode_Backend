using EvaluacionFinalMitocode_backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EvaluacionFinalMitocode_backend.Persistence
{
    // TODO: Usar esta clase para manejar la autenticacion y autorizacion de usuarios
    //public class ApplicationDbContext : IdentityDbContext<EFUserIdentity>
    public class ApplicationDbContext : DbContext

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Libro> Libros => Set<Libro>();

        // Personalizar Entidades
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Secuencia para generar IDs de Clientes y Libros
            modelBuilder.HasSequence<int>("ClienteSeq", schema: "Users").StartsAt(1).IncrementsBy(1);
            modelBuilder.HasSequence<int>("LibroSeq", schema: "Productos").StartsAt(1).IncrementsBy(1);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //modelBuilder.Entity<EFUserIdentity>(x => x.ToTable("User"));
            //modelBuilder.Entity<IdentityRole>(x => x.ToTable("Role"));
            //modelBuilder.Entity<IdentityUserRole<string>>(x => x.ToTable("UserRole"));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
