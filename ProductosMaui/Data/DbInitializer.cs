using ProductosMaui.Models;

namespace ProductosMaui.Data
{
    public class DbInitializer
    {
        public static void CrearDatos(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //crea bd y tablas si no existen
            db.Database.EnsureCreated();

            //si ya esta inicializada, no se vuelven a crear
            if (db.Productos.Any()) return;

            var seed = new List<Producto>
            {
                new() { Nombre="Mouse Óptico",       SKU="MO-100",  Precio=199.90m,  Stock=50,  Activo=true },
                new() { Nombre="Teclado Mecánico",   SKU="TM-200",  Precio=1299.00m, Stock=15,  Activo=true },
                new() { Nombre="Monitor 24",         SKU="MON-24",  Precio=2499.00m, Stock=8,   Activo=true },
                new() { Nombre="Cable HDMI 2m",      SKU="HDMI-2",  Precio=149.00m,  Stock=100, Activo=true },
                new() { Nombre="USB 64GB",           SKU="USB-64",  Precio=229.00m,  Stock=40,  Activo=true },
                new() { Nombre="Silla Ergonómica",   SKU="SE-01",   Precio=3499.00m, Stock=5,   Activo=true },
                new() { Nombre="Base Laptop",        SKU="BL-13",   Precio=399.00m,  Stock=20,  Activo=true },
                new() { Nombre="Audífonos In-Ear",   SKU="AI-10",   Precio=299.00m,  Stock=30,  Activo=true },
                new() { Nombre="Webcam 1080p",       SKU="WC-1080", Precio=699.00m,  Stock=12,  Activo=true },
                new() { Nombre="Tapete Mouse",       SKU="TM-01",   Precio=99.00m,   Stock=60,  Activo=true },
            };

            db.Productos.AddRange(seed);
            db.SaveChanges();
        }
    }
}
