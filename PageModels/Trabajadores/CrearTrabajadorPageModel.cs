using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Helpers.Interface;
using System.ComponentModel.DataAnnotations;

namespace MauiFirebase.PageModels.Trabajadores;

public partial class CrearTrabajadorPageModel : ObservableValidator
{
    [ObservableProperty]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string? nombreTrabajador;

    [ObservableProperty]
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string? apellidoTrabajador;

    [ObservableProperty]
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    public string? dniTrabajador;

    [ObservableProperty]
    public string correoTrabajador = string.Empty;

    [ObservableProperty]
    public string telefonoTrabajador = string.Empty;

    [ObservableProperty]
    public bool estadoTrabajador = true;

    [ObservableProperty]
    public int idUsuario;


    private readonly ITrabajadorRepository _trabajadorRepository;
    private readonly IAlertaHelper _alertaHelper;

    public CrearTrabajadorPageModel(ITrabajadorRepository trabajadorRepository, IAlertaHelper alertaHelper)
    {
        _trabajadorRepository = trabajadorRepository;
        _alertaHelper = alertaHelper;
    }

    [RelayCommand]
    public async Task CrearTrabajadorAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{errores}");
            return;
        }
        var nuevo = new Trabajador
        {
            NombreTrabajador = NombreTrabajador!,
            ApellidoTrabajador = ApellidoTrabajador!,
            DniTrabajador = DniTrabajador!,
            CorreoTrabajador = CorreoTrabajador,
            TelefonoTrabajador = TelefonoTrabajador,
            EstadoTrabajador = EstadoTrabajador,
            IdUsuario = IdUsuario,
            FechaRegistroTrabajador = DateTime.Now
        };
        await _trabajadorRepository.CreateTrabajadorAsync(nuevo);
        await _alertaHelper.ShowSuccessAsync("Trabajador creado correctamente.");
        await Shell.Current.GoToAsync("..");
    }
}
