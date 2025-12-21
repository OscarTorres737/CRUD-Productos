using ProductosMaui.Models;
using ProductosMaui.Services;

namespace ProductosMaui.Views;

public partial class ProductosPage : ContentPage
{
	private readonly ProductoService _service;

	private readonly List<Producto> productos = new();

    private int _skip = 0;
    private const int Take = 20;
    private int _total = 0;

    private string? _query;
    private bool _cargando = false;

    public ProductosPage(ProductoService productoService)
	{
		InitializeComponent();
        _service = productoService;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarPrimeraPagina();
    }

    private async Task CargarPrimeraPagina()
    {
        _skip = 0;
        productos.Clear();
        cvProductos.ItemsSource = null;

        await CargarMas();
    }

    private async Task CargarMas()
    {
        if (_cargando) return;

        // si ya cargó todo
        if (_total > 0 && productos.Count >= _total) return;

        _cargando = true;

        bool inactivos = swInactivos.IsToggled;
        var (items, total) = await _service.ObtenerProductos(_query, inactivos, skip: _skip, take: Take);

        _total = total;
        productos.AddRange(items);
        _skip = productos.Count;

        cvProductos.ItemsSource = null;
        cvProductos.ItemsSource = productos;

        lblInfo.Text = $"Mostrando {productos.Count} de {_total}";

        _cargando = false;
    }

    private async void CargarProductos(object sender, EventArgs e)
    {
        await CargarMas();
    }

    private async void BuscarChanged(object sender, TextChangedEventArgs e)
    {
        _query = e.NewTextValue;
        await CargarPrimeraPagina();
    }

    private async void CrearNuevo(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AgregarProducto));
    }

    private async void EditarProducto(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Producto p)
        {
            await Shell.Current.GoToAsync(nameof(DetalleProductoPage), new Dictionary<string, object>
                {
                { "ProductoSKU", p.SKU }
                });
        }
    }

    private void ProductoSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Producto producto) return;

        cvProductos.SelectedItem = null;

        DisplayAlert("Seleccionado", $"{producto.Nombre} ({producto.SKU})", "OK");
    }

    private async void BorrarProducto(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Producto p)
        {
            bool confirm = await DisplayAlert("Borrado Lógico", $"Estas seguro que deseas borrar el producto: {p.Nombre}", "Borrar", "Cancelar");

            if (!confirm)
            {
                return;
            }


            var (ok, error) = await _service.DesactivarProducto(p.Id);
            if (!ok)
            {
                await DisplayAlert("Error", error ?? "No se pudo guardar.", "OK");
                return;
            }

            await DisplayAlert("Listo", "Producto borrado.", "OK");
            await CargarPrimeraPagina();
        }
    }

    private async void Inactivos(object sender, ToggledEventArgs e)
    {
        await CargarPrimeraPagina();
    }

}