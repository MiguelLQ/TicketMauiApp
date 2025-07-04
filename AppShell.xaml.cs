﻿using CommunityToolkit.Maui.Alerts;
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
using MauiFirebase.Pages.usuario;
using MauiFirebase.Pages.Register;
//using Windows.Devices.Sensors;

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
            //para register page
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            //Usuario
            Routing.RegisterRoute(nameof(AgregarUsuarioPage), typeof(AgregarUsuarioPage));
            Routing.RegisterRoute("usuarios/agregar", typeof(AgregarUsuarioPage));

            CargarDatosUsuario();
            MostrarOpcionesSegunRol();

            //Routing.RegisterRoute("residenteForm", typeof(ResidenteFormPage));
            // En AppShell.xaml.cs

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

            //Application.Current.MainPage = new LoginPage();
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }



        private void SfSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            // 1. Cambiar el tema
            Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;

            // 2. Forzar actualización del recurso CardBackgroundBrush
            var isDark = Application.Current.RequestedTheme == AppTheme.Dark;

            Application.Current.Resources["CardBackgroundBrush"] = new SolidColorBrush(
                (Color)Application.Current.Resources[isDark ? "CardBackgroundDark" : "CardBackgroundLight"]
            );
        }
        private void CargarDatosUsuario()
        {
            try
            {
                var authService = new FirebaseAuthService();

                string correo = authService.GetUserEmail();
                string rol = Preferences.Get("FirebaseUserRole", string.Empty);

                UserEmailLabel.Text = correo;
                UserRoleLabel.Text = string.IsNullOrEmpty(rol) ? "Rol desconocido" : rol;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar datos del usuario: " + ex.Message);
            }
        }


        private void MostrarOpcionesSegunRol()
        {
            var rol = Preferences.Get("FirebaseUserRole", string.Empty);

            // Primero ocultar todo lo controlado por rol
            UsuariosFlyoutItem.IsVisible = false;
            CategoriaResiduoShellContent.IsVisible = false;
            ColoresShellContent.IsVisible = false;
            ResiduosShellContent.IsVisible = false;
            ConversionesShellContent.IsVisible = false;
            RegisterFlyoutItem.IsVisible = false;
            AdminFlyoutItem.IsVisible = false;

            // Mostrar solo lo correspondiente al rol
            if (rol == "admin")
            {
                AdminFlyoutItem.IsVisible = true;
                UsuariosFlyoutItem.IsVisible = true;
                CategoriaResiduoShellContent.IsVisible = true;
                ColoresShellContent.IsVisible = true;
                ResiduosShellContent.IsVisible = true;
                ConversionesShellContent.IsVisible = true;

            }
            else if (rol == "register")
            {
                RegisterFlyoutItem.IsVisible = true;
                AdminFlyoutItem.IsVisible = false;

            }
        }

    }
}
