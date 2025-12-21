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
        txtFechaAlta.Text = _producto.FechaAlta.ToShortDateString();
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
        var precioTexto = (txtPrecio.Text ?? "").Trim();
        var stockTexto = (txtStock.Text ?? "").Trim();
        var activo = swActivo.IsToggled;

        if (string.IsNullOrWhiteSpace(nombre))
        {
            await DisplayAlert("Validación", "El Nombre es requerido.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(precioTexto))
        {
            await DisplayAlert("Validación", "El Precio es requerido.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(stockTexto))
        {
            await DisplayAlert("Validación", "El Stock es requerido.", "OK");
            return;
        }

        if (!TryParseDecimal(precioTexto, out var precio))
        {
            await DisplayAlert("Validación", "Precio inválido. Ejemplo: 1299.00", "OK");
            return;
        }

        if (!int.TryParse(stockTexto, out var stock))
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

        //validacion para antes de guardar cambios
        bool confirm = await DisplayAlert("Confirmar Cambios", $"¿Deseas guardar los cambios de '{actualizado.Nombre}'?", "Guardar", "Cancelar");

        if (!confirm)
        {
            return;
        }

        var (ok, error) = await _service.ActualizarProducto(actualizado);
        if (!ok)
        {
            await DisplayAlert("Error", error ?? "No se pudo guardar.", "OK");
            return;
        }

        await DisplayAlert("Listo", "Producto actualizado.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async void desactivarProducto(object sender, EventArgs e)
    {
        var activo = swActivo.IsToggled;

        if (activo != true)
        {
            await DisplayAlert("Advertencia", "Al desactivar un producto y guardar cambios, el producto se borrará de manera lógica.", "De acuerdo");
        }
    }

    private void SoloNumeros(object sender, TextChangedEventArgs e)
   {
        if (string.IsNullOrEmpty(e.NewTextValue))
        {
            return;
        }

        if (!int.TryParse(e.NewTextValue, out _))
        {
            ((Entry)sender).Text = e.OldTextValue;
        }
    }

    private void SoloDecimal(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        var text = e.NewTextValue;

        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        //solo num y separador decimal
        foreach (var ch in text)
        {
            if (!char.IsDigit(ch) && ch != '.' && ch != ',')
            {
                entry.Text = e.OldTextValue;
                return;
            }
        }

        //solo un separador decimal
        var dots = text.Count(c => c == '.');
        var commas = text.Count(c => c == ',');
        if (dots + commas > 1)
        {
            entry.Text = e.OldTextValue;
            return;
        }

        //evita que empiece con separador (".5" o ",5")
        if (text.Length == 1 && (text[0] == '.' || text[0] == ','))
        {
            entry.Text = e.OldTextValue;
            return;
        }

        //limita a 2 decimales
        var sepIndex = text.IndexOfAny(new[] { '.', ',' });
        if (sepIndex >= 0)
        {
            var decimals = text[(sepIndex + 1)..];
            if (decimals.Length > 2)
            {
                entry.Text = e.OldTextValue;
                return;
            }
        }
    }

    private static bool TryParseDecimal(string? input, out decimal value)
    {
        input = (input ?? "").Trim();
        value = 0m;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var lastDot = input.LastIndexOf('.');
        var lastComma = input.LastIndexOf(',');

        if (lastDot >= 0 && lastComma >= 0)
        {
            if (lastDot > lastComma)
            {
                input = input.Replace(",", "");
            }
            else
            {
                input = input.Replace(".", "");
                input = input.Replace(',', '.');
            }
        }
        else if (lastComma >= 0)
        {
            input = input.Replace(".", "");
            input = input.Replace(',', '.');
        }
        else
        {
            input = input.Replace(",", "");
        }

        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

}