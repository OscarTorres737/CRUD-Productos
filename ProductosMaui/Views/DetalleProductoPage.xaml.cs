using ProductosMaui.Models;
using ProductosMaui.Services;
using System.Globalization;

namespace ProductosMaui.Views;

[QueryProperty(nameof(ProductoSKU), "ProductoSKU")]
public partial class DetalleProductoPage : ContentPage
{
	private readonly ProductoService _service;
	private Producto? _producto;

    public string ProductoSKU { get; set; }

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

        var sku = (ProductoSKU ?? "").Trim();

        if (string.IsNullOrWhiteSpace(sku))
        {
            await DisplayAlert("Error", "SKU inválido.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        _producto = await _service.ObtenerPorSKU(sku);

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
        var activo = swActivo.IsToggled;
        decimal precio = decimal.Parse(txtPrecio.Text);
        int stock = Int32.Parse(txtStock.Text);


        if (!TryParseDecimal(txtPrecio.Text, out var presio))
        {
            await DisplayAlert("Validación", "Precio inválido. Ejemplo: 1299.00", "OK");
            return;
        }

        if (!int.TryParse((txtStock.Text ?? "").Trim(), out var stok))
        {
            await DisplayAlert("Validación", "Stock inválido. Ejemplo: 20", "OK");
            return;
        }

        //armado de obj
        var actualizado = new Producto
        {
            Id = _producto.Id,
            SKU = _producto.SKU,
            Nombre = nombre,
            Precio = precio,
            Stock = stock,
            Activo = activo,
            FechaAlta = _producto.FechaAlta
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