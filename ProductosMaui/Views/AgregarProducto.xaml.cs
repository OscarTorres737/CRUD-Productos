using ProductosMaui.Models;
using ProductosMaui.Services;
using System.Globalization;

namespace ProductosMaui.Views;

public partial class AgregarProducto : ContentPage
{
    private readonly ProductoService _productoService;
    public AgregarProducto(ProductoService service)
    {
        _productoService = service;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        txtFechaAlta.Text = DateTime.UtcNow.ToString("dddd dd/MM/yyyy");
    }

    private async void Cancelar(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void Agregar(object sender, EventArgs e)
    {
        var SKU = (txtSKU.Text ?? "").Trim().ToUpperInvariant();
        var nombre = (txtNombre.Text ?? "").Trim();
        var precioTexto = (txtPrecio.Text ?? "").Trim();
        var stockTexto = (txtStock.Text ?? "").Trim();

        if (string.IsNullOrWhiteSpace(SKU))
        {
            await DisplayAlert("Validación", "El SKU es requerido.", "OK");
            return;
        }

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

        var nuevo = new Producto
        {
            SKU = SKU,
            Nombre = nombre,
            Precio = precio,
            Stock = stock,
        };

        bool confirm = await DisplayAlert("Confirmar Producto", $"¿Deseas agregar el producto '{nuevo.Nombre}'?", "Agregar", "Cancelar");

        if (!confirm)
        {
            return;
        }

        var (ok, error) = await _productoService.CrearProducto(nuevo);
        if (!ok)
        {
            await DisplayAlert("Error", error ?? "No se pudo agregar el producto.", "OK");
            return;
        }

        await DisplayAlert("Listo", "Producto agregado.", "OK");
        await Shell.Current.GoToAsync("..");
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