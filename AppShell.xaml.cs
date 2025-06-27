using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MauiFirebase.Pages.Canje;
using MauiFirebase.Pages.Premio;
using MauiFirebase.Pages.ResidentesView;
using MauiFirebase.Pages.Residuo;
using MauiFirebase.Pages.Ticket;
using MauiFirebase.Pages.RegistroDeReciclaje;
using Font = Microsoft.Maui.Font;
using MauiFirebase.Pages.Convertidores;

using MauiFirebase.Pages.Login;

namespace MauiFirebase
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();
      
            Routing.RegisterRoute(nameof(EditarResiduoPage), typeof(EditarResiduoPage));
            Routing.RegisterRoute(nameof(AgregarResiduoPage), typeof(AgregarResiduoPage));

            Routing.RegisterRoute(nameof(EditarConvertidorPage), typeof(EditarConvertidorPage));
            Routing.RegisterRoute(nameof(AgregarConvertidorPage), typeof(AgregarConvertidorPage));

            Routing.RegisterRoute(nameof(AgregarPremioPage), typeof(AgregarPremioPage));
            Routing.RegisterRoute(nameof(EditarPremioPage), typeof(EditarPremioPage));
            Routing.RegisterRoute(nameof(EditarTicketPage), typeof(EditarTicketPage));

            // Registro de rutas de reciclaje
            Routing.RegisterRoute(nameof(AgregarRegistroPage), typeof(AgregarRegistroPage));
            Routing.RegisterRoute(nameof(ListarRegistrosPage), typeof(ListarRegistrosPage));

            Routing.RegisterRoute(nameof(AgregarCanjePage), typeof(AgregarCanjePage));
            Routing.RegisterRoute(nameof(EditarCanjePage), typeof(EditarCanjePage));
            Routing.RegisterRoute("residenteForm", typeof(ResidenteFormPage));

            Routing.RegisterRoute("LoginPage", typeof(LoginPage));

            // Escuchar cambio de rutas
            Navigated += AppShell_Navigated;
            var currentTheme = Application.Current!.RequestedTheme;
            ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;
        }
        public static async Task DisplaySnackbarAsync(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#FF3300"),
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(0),
                Font = Font.SystemFontOfSize(18),
                ActionButtonFont = Font.SystemFontOfSize(14)
            };

            var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
        }

        public static async Task DisplayToastAsync(string message)
        {
            // Toast is currently not working in MCT on Windows
            if (OperatingSystem.IsWindows())
                return;

            var toast = Toast.Make(message, textSize: 18);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await toast.Show(cts.Token);
        }
        private void AppShell_Navigated(object sender, ShellNavigatedEventArgs e)
        {
            // Detectar si estás en la página de login
            if (e.Current.Location.OriginalString.Contains("LoginPage"))
            {
                Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
                Shell.SetNavBarIsVisible(this, false); // Oculta barra de navegación
            }
            else
            {
                Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
                Shell.SetNavBarIsVisible(this, true);
            }
        }
        private void CerrarSesion_Clicked(object sender, EventArgs e)
        {
            var authService = new FirebaseAuthService();
            authService.Logout();

            Application.Current.MainPage = new LoginPage();
        }



        private void SfSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
        }
    }
}
