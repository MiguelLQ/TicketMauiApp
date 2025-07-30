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
        MainPage = new ContentPage
        {
            Content = new ActivityIndicator
            {
                IsRunning = true,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }
        };
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await InicializarAplicacionCompletaAsync();
        });
    }

    private async Task InicializarAplicacionCompletaAsync()
    {
        var authService = new FirebaseAuthService();
        //Mostrar Onboarding si aún no se ha visto
        bool onboardingVisto = Preferences.Get("onboarding_visto", false);
        if (!onboardingVisto)
        {
            var onboarding = new OnboardingPage();

            onboarding.CuandoFinaliza = () =>
            {
                Preferences.Set("onboarding_visto", true);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await InicializarAplicacionCompletaAsync(); // vuelve a ejecutar el flujo normal
                });
            };

            MainPage = new NavigationPage(onboarding);
            return;
        }


        // Intenta restaurar sesión siempre, no solo si está "loggeado"
        await RestaurarSesionAsync(authService);

        //Sincronización si hay internet
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
            var shell = new AppShell();
            MainPage = shell;

            // Esperar hasta que Shell esté completamente inicializado
            await Task.Delay(300); // A veces 200 ms no es suficiente

            shell.MostrarOpcionesSegunRol();

            // Usa Dispatcher para asegurar hilo UI
            MainThread.BeginInvokeOnMainThread(async () =>
            {
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
                    default:
                        await Shell.Current.GoToAsync("//ciudadanoHome/inicioCiudadano");
                        break;
                }
            });
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

    //public static async Task BorrarBaseDeDatosLocalAsync()
    //{
    //    var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app2.db3");

    //    if (File.Exists(dbPath))
    //    {
    //        File.Delete(dbPath);
    //    }

    //    var nuevaBD = new AppDatabase(dbPath);
    //}

    protected override void CleanUp()
    {
        Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        base.CleanUp();
    }
}
