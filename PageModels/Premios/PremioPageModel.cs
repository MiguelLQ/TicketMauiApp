using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using MauiFirebase.Pages.Premio;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Premios;

public partial class PremioPageModel : ObservableObject
{
    public ObservableCollection<Premio> ListaPremios { get; } = new();

    private readonly IPremioRepository _premioRepository;
    private readonly IAlertaHelper _alertaHelper;

    [ObservableProperty]
    private bool _isBusy;
  

    public PremioPageModel(IPremioRepository premioRepository, IAlertaHelper alertaHelper)
    {
        _alertaHelper = alertaHelper;

        _premioRepository = premioRepository;
    }
    [ObservableProperty]
    private bool esAdmin;

    public void VerificarRol()
    {
        var rol = Preferences.Get("FirebaseUserRole", string.Empty);
        EsAdmin = rol == "Administrador";

        System.Diagnostics.Debug.WriteLine($"👮 ROL detectado en ViewModel: {rol} - ¿EsAdmin?: {EsAdmin}");
    }



    partial void OnEsAdminChanged(bool oldValue, bool newValue)
    {
        OnPropertyChanged(nameof(EsAdmin)); // Forzar el refresco
    }


    [RelayCommand]
    public async Task CargarPremiosAsync()
    {
        try
        {
            IsBusy = true;
            ListaPremios.Clear(); // Limpiamos primero
            var premios = await _premioRepository.GetAllPremiosAsync(); // Obtenemos desde BD
            foreach (var p in premios)
            {
                ListaPremios.Add(p);
            }
        }
        finally
        {
            IsBusy = false; 
        }
    }


    [RelayCommand]
    public async Task CambiarEstadoPremioAsync(int id)
    {
        await _premioRepository.ChangePremioStatusAsync(id);
        await CargarPremiosAsync();
    }

    [RelayCommand]
    public async Task IrAEditarPremioAsync(Premio premio)
    {
        var parametros = new Dictionary<string, object>
        {
            { "id", premio.IdPremio }
        };

        await Shell.Current.GoToAsync($"{nameof(EditarPremioPage)}?id={premio.IdPremio}");
    }
    [RelayCommand]
    public async Task SincronizarPremiosDesdeFirestoreAsync()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await _alertaHelper.ShowWarningAsync("Para sincronizar premios disponibles , necesitas conexion a internet.");
            return;
        }

        try
        {
            IsBusy = true;

            // 🔒 Token de Firebase
            var idToken = Preferences.Get("FirebaseToken", string.Empty);
            if (string.IsNullOrWhiteSpace(idToken))
            {
                await _alertaHelper.ShowErrorAsync("Error al obtener token");
                return;
            }

            // 🔍 Consultar desde Firestore
            var firebaseService = new FirebasePremioService();
            var premiosFirestore = await firebaseService.ObtenerPremiosDesdeFirestoreAsync(idToken);

            // Guardar o actualizar en SQLite
            foreach (var premioRemoto in premiosFirestore)
            {
                var existente = await _premioRepository.GetPremioByIdAsync(premioRemoto.IdPremio);

                // 🖼️ Verificar si se necesita descargar imagen
                if (!string.IsNullOrEmpty(premioRemoto.FotoPremioUrl))
                {
                    var imagenYaExiste = !string.IsNullOrEmpty(existente?.FotoPremio) && File.Exists(existente.FotoPremio);
                    if (imagenYaExiste)
                    {
                        premioRemoto.FotoPremio = existente.FotoPremio; // usar la imagen local
                    }
                    else
                    {
                        premioRemoto.FotoPremio = await _premioRepository.DescargarImagenLocalAsync(premioRemoto.FotoPremioUrl);
                    }
                }

                if (existente == null)
                {
                    await _premioRepository.CreatePremioLocalAsync(premioRemoto);
                }
                else
                {
                    await _premioRepository.UpdatePremioAsync(premioRemoto);
                }
            }

            await CargarPremiosAsync(); // refrescar lista visible
            await _alertaHelper.ShowSuccessAsync("premios disponibles sincornizados correctamente");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "Cerrar");
        }
        finally
        {
            IsBusy = false;
        }
    }



    [RelayCommand]
    public async Task IrACrearPremioAsync()
    {
        await Shell.Current.GoToAsync(nameof(AgregarPremioPage));
    }
}
