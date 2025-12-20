using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductosMaui.Data;
using ProductosMaui.Services;

namespace ProductosMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            //ruta y nombre de la bd
            var dbpath = Path.Combine(FileSystem.AppDataDirectory, "productos.db");
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data source={dbpath}"));

            builder.Services.AddTransient<ProductoService>();
            builder.Services.AddTransient<Views.ProductosPage>();

            var app = builder.Build();
            

            DbInitializer.CrearDatos(app.Services);

            return app;
            
        }
    }
}
