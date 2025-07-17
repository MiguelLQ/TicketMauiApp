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

        InitializeAppAsync();
    }

    private async void InitializeAppAsync()
    {
        var authService = new FirebaseAuthService();

        // Solo en el primer inicio
        if (!Preferences.ContainsKey("PrimeraEjecucion"))
        {
            Preferences.Set("PrimeraEjecucion", true);
            await BorrarBaseDeDatosLocalAsync();
        }

        if (authService.IsLoggedIn())
        {
            await authService.ObtenerIdTokenSeguroAsync();
            MainPage = new AppShell();
        }
        else
        {
            MainPage = new NavigationPage(new LoginPage());
        }

        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            await IntentarSincronizarAsync();
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
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");

        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        // Recrear la base de datos
        var nuevaBD = new AppDatabase(dbPath);
    }

    protected override void CleanUp()
    {
        Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        base.CleanUp();
    }
}
