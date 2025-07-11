using MauiFirebase.PageModels.CamScaners;
using ZXing.Net.Maui;
using System.Text.RegularExpressions;
using MauiFirebase.Data.Interfaces;

namespace MauiFirebase.Pages.CamScaner;

public partial class CamScanerPage : ContentPage
{
    private readonly CamScanerPageModel _viewModel;
    private bool _hasScanned = false;

    public CamScanerPage(IResidenteRepository residenteRepository)
    {
        InitializeComponent();
        _viewModel = new CamScanerPageModel(residenteRepository);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _hasScanned = false;
        barcodeReader.IsDetecting = true;
    }

    private async void barcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_hasScanned) return;

        var result = e.Results?.FirstOrDefault();
        if (result == null) return;

        _hasScanned = true;
        barcodeReader.IsDetecting = false;

        await Dispatcher.DispatchAsync(async () =>
        {
            string qrValue = result.Value;
            _viewModel.CodigoDetectado = qrValue;

            var match = Regex.Match(qrValue, @"DNI[:\s]*([0-9]{8})");

            if (!match.Success)
            {
                await Shell.Current.DisplayAlert("Error", "No se encontró un DNI válido en el código QR.", "OK");
                ReiniciarEscaner();
                return;
            }

            string dni = match.Groups[1].Value;

            // ✅ Validar si el DNI existe
            bool existe = await _viewModel.ValidarDniExistenteAsync(dni);

            if (!existe)
            {
                await Shell.Current.DisplayAlert("No Registrado", "El ciudadano no está registrado en el sistema.", "OK");
                ReiniciarEscaner();
                return;
            }

            // ✅ Navegar solo si el DNI existe
            await Shell.Current.GoToAsync($"AgregarRegistroPage?dni={dni}");
        });
    }

    private void ReiniciarEscaner()
    {
        _hasScanned = false;
        barcodeReader.IsDetecting = true;
    }
}
