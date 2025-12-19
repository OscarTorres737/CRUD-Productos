using Microsoft.EntityFrameworkCore;
using ProductosMaui.Models;

namespace ProductosMaui.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Producto> Productos => Set<Producto>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var p = modelBuilder.Entity<Producto>();

            p.ToTable("Productos");
            p.HasKey(x => x.Id);

            p.HasIndex(x => x.SKU).IsUnique();
            p.Property(x => x.Precio).HasPrecision(10, 2);

            p.Property(x => x.Activo).HasDefaultValue(true);
        }


        public override int SaveChanges()
        {
            SetFechaAlta();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetFechaAlta();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetFechaAlta()
        {
            var fecha = DateTime.UtcNow;

            foreach (var e in ChangeTracker.Entries<Producto>())
            {
                if (e.State == EntityState.Added && e.Entity.FechaAlta == default)
                    e.Entity.FechaAlta = fecha;
            }
        }

    }
}
