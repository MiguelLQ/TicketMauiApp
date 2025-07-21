using MauiFirebase.PageModels.Residentes;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using System;

namespace MauiFirebase.Pages.Home
{
    public partial class inicioCiudadanoPage : ContentPage
    {
        private readonly InicioCiudadanoPageModel _viewModel;

        // Para arrastrar el botón
        double xOffset, yOffset;

        public inicioCiudadanoPage(InicioCiudadanoPageModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                Shell.SetBackgroundColor(this, Color.FromArgb("#3b46d6"));
                Shell.SetForegroundColor(this, Colors.White);

                await Task.Delay(300); // Espera para pintar la UI
                _viewModel.IsBusy = true;
                await _viewModel.CargarDatosUsuarioAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Se produjo un error: {ex.Message}", "OK");
                Console.WriteLine($"🔴 OnAppearing Error: {ex}");
            }
            finally
            {
                _viewModel.IsBusy = false;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            Shell.SetBackgroundColor(this, Application.Current!.RequestedTheme == AppTheme.Dark ? Colors.Black : Colors.White);
            Shell.SetTitleColor(this, Colors.Black);
            Shell.SetForegroundColor(this, Colors.Black);
        }

        // Botón WhatsApp: redirige al enlace
        private async void OnWhatsAppClicked(object sender, EventArgs e)
        {
            string numeroTelefono = "51920805556";
            string mensaje = Uri.EscapeDataString("Hola, me gustaría reportar un incidente.");
            string url = $"https://wa.me/{numeroTelefono}?text={mensaje}";

            try
            {
                await Launcher.Default.OpenAsync(url);
            }
            catch
            {
                await DisplayAlert("Error", "No se pudo abrir WhatsApp.", "OK");
            }
        }

        // Lógica para mover el botón con el dedo (drag & drop)
        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    // Guardar posición inicial
                    xOffset = BotonWsp.TranslationX;
                    yOffset = BotonWsp.TranslationY;
                    break;

                case GestureStatus.Running:
                    // Mover el botón mientras se arrastra
                    BotonWsp.TranslationX = xOffset + e.TotalX;
                    BotonWsp.TranslationY = yOffset + e.TotalY;
                    break;

                case GestureStatus.Completed:
                    // Podrías guardar la posición aquí si lo deseas
                    break;
            }
        }
    }
}
