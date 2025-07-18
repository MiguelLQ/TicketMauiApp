using MauiFirebase.PageModels.Residentes;
using Microsoft.Maui.Controls;

namespace MauiFirebase.Pages.Home
{
    public partial class inicioCiudadanoPage : ContentPage
    {
        private readonly InicioCiudadanoPageModel _viewModel;

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
                Shell.SetBackgroundColor(this, Color.FromArgb("#5061c8"));
                Shell.SetForegroundColor(this, Colors.White);

                await Task.Delay(300); // Deja tiempo para pintar la UI

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

        private async void OnWhatsAppClicked(object sender, EventArgs e)
        {
            string numeroTelefono = "51987654321";
            string mensaje = Uri.EscapeDataString("Hola, me gustaría obtener más información.");
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
    }
}
