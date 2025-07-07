using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Helpers.Interface;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Trabajadores;

[QueryProperty(nameof(IdTrabajador), "id")]
public partial class EditarTrabajadorPageModel : ObservableValidator
{
    private readonly ITrabajadorRepository _trabajadorRepository;
    private readonly IAlertaHelper _alertaHelper;

    [ObservableProperty]
    private int idTrabajador;

    [ObservableProperty]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    private string? nombreTrabajador;

    [ObservableProperty]
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    private string? apellidoTrabajador;

    [ObservableProperty]
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    private string? dniTrabajador;

    [ObservableProperty]
    private string correoTrabajador = string.Empty;

    [ObservableProperty]
    private string telefonoTrabajador = string.Empty;

    [ObservableProperty]
    private bool estadoTrabajador;

    [ObservableProperty]
    private int idUsuario;

    public EditarTrabajadorPageModel(ITrabajadorRepository trabajadorRepository, IAlertaHelper alertaHelper)
    {
        _trabajadorRepository = trabajadorRepository;
        _alertaHelper = alertaHelper;
    }

    public async Task InicializarAsync()
    {
        var trabajador = await _trabajadorRepository.GetTrabajadorByIdAsync(IdTrabajador);
        if (trabajador != null)
        {
            NombreTrabajador = trabajador.NombreTrabajador;
            ApellidoTrabajador = trabajador.ApellidoTrabajador;
            DniTrabajador = trabajador.DniTrabajador;
            CorreoTrabajador = trabajador.CorreoTrabajador;
            TelefonoTrabajador = trabajador.TelefonoTrabajador;
            EstadoTrabajador = trabajador.EstadoTrabajador;
            IdUsuario = trabajador.IdUsuario;
        }
    }

    [RelayCommand]
    public async Task EditarTrabajadorAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{errores}");
            return;
        }
        var actualizado = new Trabajador
        {
            IdTrabajador = IdTrabajador,
            NombreTrabajador = NombreTrabajador!,
            ApellidoTrabajador = ApellidoTrabajador!,
            DniTrabajador = DniTrabajador!,
            CorreoTrabajador = CorreoTrabajador,
            TelefonoTrabajador = TelefonoTrabajador,
            EstadoTrabajador = EstadoTrabajador,
            IdUsuario = IdUsuario,
            FechaRegistroTrabajador = DateTime.Now
        };
        await _trabajadorRepository.UpdateTrabajadorAsync(actualizado);
        await _alertaHelper.ShowSuccessAsync("Trabajador actualizado correctamente.");
        await Shell.Current.GoToAsync("..");
    }
}
