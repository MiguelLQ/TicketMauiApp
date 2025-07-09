using MauiFirebase.PageModels.CamScaners;
using ZXing;
using ZXing.Net.Maui;


namespace MauiFirebase.Pages.CamScaner;

public partial class CamScanerPage : ContentPage
{
    private readonly CamScanerPageModel _viewModel;
    public CamScanerPage()
    {
        InitializeComponent();
        _viewModel = new CamScanerPageModel();
        BindingContext = _viewModel;
    }
    private async void barcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var first = e.Results?.FirstOrDefault();
        if (first == null) return;

        barcodeReader.IsDetecting = false;

        await Dispatcher.DispatchAsync(async () =>
        {
            // Extraer solo el número de DNI desde el texto escaneado
            string qrValue = first.Value;

            // Buscar un DNI dentro del texto (8 dígitos)
            var match = System.Text.RegularExpressions.Regex.Match(qrValue, @"\b\d{8}\b");

            if (!match.Success)
            {
                await Shell.Current.DisplayAlert("Error", "No se encontró un DNI válido en el código QR.", "OK");
                barcodeReader.IsDetecting = true;
                return;
            }

            string dni = match.Value;

            // Navegar pasando solo el DNI limpio
            await Shell.Current.GoToAsync($"AgregarCanjePage?dni={dni}");

            barcodeReader.IsDetecting = true;
        });
    }
}
