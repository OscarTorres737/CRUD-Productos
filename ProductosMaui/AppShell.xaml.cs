using ProductosMaui.Views;

namespace ProductosMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(DetalleProductoPage), typeof(DetalleProductoPage));
            Routing.RegisterRoute(nameof(AgregarProducto), typeof(AgregarProducto));
        }
    }
}
