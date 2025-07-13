using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace MauiFirebase.PageModels.Premios;

[QueryProperty(nameof(IdPremio), "id")]
public partial class EditarPremioPageModel : ObservableObject
{
    private readonly IPremioRepository _premioRepository;
    private readonly IAlertaHelper _alertaHelper;

    [ObservableProperty]
    private int idPremio;

    [ObservableProperty]
    private string tituloPagina = "Editar Premio";

    [ObservableProperty]
    private string? nombrePremio;

    [ObservableProperty]
    private string? descripcionPremio;

    [ObservableProperty]
    private int puntosRequeridos;

    [ObservableProperty]
    private bool estadoPremio;

    [ObservableProperty]
    private Premio? premioSeleccionado;

    [ObservableProperty]
    private string? fotoPremio;


    public EditarPremioPageModel(IPremioRepository premioRepository, IAlertaHelper alertaHelper)
    {
        _premioRepository = premioRepository;
        _alertaHelper = alertaHelper;
    }

    public async Task InicializarAsync()
    {
        PremioSeleccionado = await _premioRepository.GetPremioByIdAsync(IdPremio);

        if (PremioSeleccionado != null)
        {
            NombrePremio = PremioSeleccionado.NombrePremio;
            DescripcionPremio = PremioSeleccionado.DescripcionPremio;
            PuntosRequeridos = PremioSeleccionado.PuntosRequeridos;
            EstadoPremio = PremioSeleccionado.EstadoPremio;
            FotoPremio = PremioSeleccionado.FotoPremio ?? string.Empty; // ✅ se carga la foto si existe
        }
    }

    [RelayCommand]
    public async Task SeleccionarImagenAsync()
    {
        var resultado = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Selecciona una imagen"
        });

        if (resultado != null)
        {
            FotoPremio = resultado.FullPath;
        }
    }

    [RelayCommand]
    public async Task GuardarCambiosAsync()
    {
        if (PremioSeleccionado == null)
            return;

        // Validar campos obligatorios
        if (string.IsNullOrWhiteSpace(NombrePremio) ||
            string.IsNullOrWhiteSpace(DescripcionPremio) ||
            PuntosRequeridos <= 0)
        {
            await _alertaHelper.ShowWarningAsync("Completa todos los campos correctamente.");
            return;
        }

        // Validar conexión a Internet para sincronizar
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await _alertaHelper.ShowWarningAsync("Necesitas conexión a Internet para sincronizar los cambios.");
            return;
        }

        // 🔁 Actualizar propiedades
        PremioSeleccionado.NombrePremio = NombrePremio;
        PremioSeleccionado.DescripcionPremio = DescripcionPremio;
        PremioSeleccionado.PuntosRequeridos = PuntosRequeridos;
        PremioSeleccionado.EstadoPremio = EstadoPremio;

        // ⚠️ Si cambiaste la imagen (ruta local nueva), puedes subir una nueva a Supabase
        if (!string.IsNullOrEmpty(FotoPremio) && FotoPremio != PremioSeleccionado.FotoPremio)
        {
            var storage = new SupabaseStorageService();
            using var stream = File.OpenRead(FotoPremio);
            var nombreRemoto = $"premios/{Guid.NewGuid()}{Path.GetExtension(FotoPremio)}";

            var nuevaUrl = await storage.SubirImagenAsync(stream, nombreRemoto);
            if (!string.IsNullOrEmpty(nuevaUrl))
            {
                PremioSeleccionado.FotoPremioUrl = nuevaUrl;
                PremioSeleccionado.FotoPremio = FotoPremio;
            }
        }

        // 🔄 Actualizar local (SQLite)
        await _premioRepository.UpdatePremioAsync(PremioSeleccionado);

        // 🔄 Actualizar en Firestore
        var idToken = await new FirebaseAuthService().ObtenerIdTokenSeguroAsync();
        var actualizado = await new FirebasePremioService().GuardarPremioFirestoreAsync(PremioSeleccionado, PremioSeleccionado.IdPremio.ToString(), idToken);

        if (actualizado)
            await _alertaHelper.ShowSuccessAsync("Premio actualizado correctamente.");
        else
            await _alertaHelper.ShowWarningAsync("Premio actualizado localmente, pero no se sincronizó con Firestore.");

        await Shell.Current.GoToAsync("..");
    }

}
