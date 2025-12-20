using ProductosMaui.Models;
using ProductosMaui.Services;
using System.Globalization;

namespace ProductosMaui.Views;

[QueryProperty(nameof(Producto), "ProductoSKU")]
public partial class DetalleProductoPage : ContentPage
{
	private readonly ProductoService _service;

	private string SKU;
	private Producto? _producto;

	public string ProductoId { get; set; } = string.Empty;

	public DetalleProductoPage(ProductoService productoService)
	{
		InitializeComponent();
        _service = productoService;
    }

	protected override async void OnAppearing()
	{
        base.OnAppearing();

        if (_producto is not null)
        {
            return;
        }

        _producto = await _service.ObtenerPorSKU(SKU);

        if (_producto is null)
        {
            await DisplayAlert("Error", "Producto no encontrado.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        // cargar en UI
        txtNombre.Text = _producto.Nombre;
        txtSku.Text = _producto.SKU;
        txtPrecio.Text = _producto.Precio.ToString("0.00", CultureInfo.InvariantCulture);
        txtStock.Text = _producto.Stock.ToString(CultureInfo.InvariantCulture);
        swActivo.IsToggled = _producto.Activo;
    }

    private async void Cancelar(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void Guardar(object sender, EventArgs e)
    {
        if (_producto is null) return;

        var nombre = (txtNombre.Text ?? "").Trim();
        var sku = _producto.SKU; // NO editable
        var activo = swActivo.IsToggled;

        if (!TryParseDecimal(txtPrecio.Text, out var precio))
        {
            await DisplayAlert("Validación", "Precio inválido. Ejemplo: 1299.00", "OK");
            return;
        }

        if (!int.TryParse((txtStock.Text ?? "").Trim(), out var stock))
        {
            await DisplayAlert("Validación", "Stock inválido. Ejemplo: 20", "OK");
            return;
        }

        //armado de obj
        var actualizado = new Producto
        {
            Nombre = nombre,
            Precio = precio,
            Stock = stock,
            Activo = activo,
        };

        var (ok, error) = await _service.ActualizarProducto(actualizado);
        if (!ok)
        {
            await DisplayAlert("Error", error ?? "No se pudo guardar.", "OK");
            return;
        }

        await DisplayAlert("Listo", "Producto actualizado.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private static bool TryParseDecimal(string? input, out decimal value)
    {
        input = (input ?? "").Trim();

        if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value))
            return true;

        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }
}