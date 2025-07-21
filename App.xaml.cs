using MauiFirebase.Data.Sources;
using MauiFirebase.Pages.Login;
using Microsoft.Maui.Networking;

namespace MauiFirebase;

public partial class App : Application
{
    private readonly SincronizacionFirebaseService _sincronizador;
    private bool _estaSincronizando = false;

    public App(SincronizacionFirebaseService sincronizador)
    {
        InitializeComponent();
        _sincronizador = sincronizador;
        Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await InicializarAplicacionCompletaAsync();
        });
    }

    private async Task InicializarAplicacionCompletaAsync()
    {
        var authService = new FirebaseAuthService();

        // Solo la primera vez
        if (!Preferences.ContainsKey("PrimeraEjecucion"))
        {
            Preferences.Set("PrimeraEjecucion", true);
            await BorrarBaseDeDatosLocalAsync();
        }

        if (authService.IsLoggedIn())
        {
            // Si hay una sesión activa, intenta restaurarla con rol
            await RestaurarSesionAsync(authService);
        }
        else
        {
            // No hay sesión activa, ir al login
            MainPage = new NavigationPage(new LoginPage());
        }

        // Intentar sincronizar si hay internet
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            await IntentarSincronizarAsync();
        }
    }

    private async Task RestaurarSesionAsync(FirebaseAuthService authService)
    {
        bool sesionActiva = await authService.IntentarRestaurarSesionAsync();

        if (sesionActiva)
        {
            string rol = Preferences.Get("FirebaseUserRole", string.Empty);

            // Cargar el AppShell
            MainPage = new AppShell();

            await Task.Delay(200); // Espera para evitar errores de navegación prematura

            ((AppShell)MainPage).MostrarOpcionesSegunRol();

            // Navegar según rol
            switch (rol)
            {
                case "Administrador":
                    await Shell.Current.GoToAsync("//adminHome/inicio");
                    break;
                case "Recolector":
                    await Shell.Current.GoToAsync("//registerHome/inicio");
                    break;
                case "Conductor":
                    await Shell.Current.GoToAsync("//conductorHome/inicioConductor");
                    break;
                case "Ciudadano":
                default:
                    await Shell.Current.GoToAsync("//ciudadanoHome/inicioCiudadano");
                    break;
            }
        }
        else
        {
            MainPage = new NavigationPage(new LoginPage());
        }
    }

    private async Task IntentarSincronizarAsync()
    {
        if (_estaSincronizando) return;

        _estaSincronizando = true;
        try
        {
            await _sincronizador.SincronizarTodoAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Error al sincronizar: " + ex.Message);
        }
        finally
        {
            _estaSincronizando = false;
        }
    }

    private async void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            await IntentarSincronizarAsync();
        }
    }

    public static async Task BorrarBaseDeDatosLocalAsync()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app2.db3");

        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        var nuevaBD = new AppDatabase(dbPath);
    }

    protected override void CleanUp()
    {
        Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        base.CleanUp();
    }
}
