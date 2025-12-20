using Microsoft.EntityFrameworkCore;
using ProductosMaui.Data;
using ProductosMaui.Models;

namespace ProductosMaui.Services
{
    public class ProductoService
    {
        private readonly AppDbContext _db;

        public ProductoService (AppDbContext db)
        {
            _db = db;
        }
       
        public async Task<(List<Producto> Items, int Total)> ObtenerProductos(string? query, bool incluirInactivos, int skip, int take)
        {
            var q = _db.Productos.AsNoTracking().AsQueryable();

            if (!incluirInactivos)
            {
                q = q.Where(x => x.Activo);
            }
                
            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim();
                q = q.Where(x => x.Nombre.Contains(query) || x.SKU.Contains(query));
            }

            var total = await q.CountAsync();

            var items = await q.OrderBy(x => x.Nombre)
                               .Skip(skip)
                               .Take(take)
                               .ToListAsync();

            return (items, total);
        }

        public Task<Producto?> ObtenerPorId(int id)
        {
            return _db.Productos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(bool Ok, string? Error)> CrearProducto(Producto producto)
        {
            var error = await ValidarProducto(0, producto);
            if (error is not null)
            {
                return (false, error);
            }

            producto.Nombre = producto.Nombre.Trim();
            producto.SKU = producto.SKU.Trim();

            _db.Productos.Add(producto);

            try
            {
                await _db.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "El SKU ya existe. Debe ser único.");
            }
        }

        public async Task<(bool Ok, string? Error)> ActualizarProducto(Producto producto)
        {
            var error = await ValidarProducto(producto.Id, producto);
            if (error is not null)
            {
                return (false, error);
            }

            var actual = await _db.Productos.FirstOrDefaultAsync(x => x.Id == producto.Id);

            if (actual is null)
            {
                return (false, "Producto no encontrado.");
            }

            actual.Nombre = producto.Nombre.Trim();
            actual.SKU = producto.SKU.Trim();
            actual.Precio = producto.Precio;
            actual.Stock = producto.Stock;
            actual.Activo = producto.Activo;

            try
            {
                await _db.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "El SKU ya existe.");
            }
        }

        public async Task<(bool Ok, string? Error)> DesactivarProducto(int id)
        {
            var p = await _db.Productos.FirstOrDefaultAsync(x => x.Id == id);
            if (p is null)
            {
                return (false, "Producto no encontrado.");
            }

            p.Activo = false;
            await _db.SaveChangesAsync();
            return (true, null);
        }

        private async Task<string?> ValidarProducto(int id, Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre))
            {
                return "El Nombre es requerido.";
            }
                
            if (producto.Nombre.Trim().Length > 100)
            {
                return "El Nombre no puede exceder 100 caracteres.";
            }
                
            if (string.IsNullOrWhiteSpace(producto.SKU))
            {
                return "El SKU es requerido.";
            }
               
            if (producto.SKU.Trim().Length > 30)
            {
                return "El SKU no puede exceder 30 caracteres.";
            }
                
            if (producto.Precio < 0)
            {
                return "El Precio no puede ser negativo.";
            }
                
            if (producto.Stock < 0)
            {
                return "El Stock no puede ser negativo.";
            }
                
            var sku = producto.SKU.Trim();
            var existe = await _db.Productos.AnyAsync(x => x.SKU == sku && x.Id != id);
            if (existe)
            {
                return "El SKU ya existe.";
            }
                
            return null;
        }

    }
}
