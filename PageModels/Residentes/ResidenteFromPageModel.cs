using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Helpers.Interface;


namespace MauiFirebase.PageModels.Residentes
{
    [QueryProperty(nameof(IdResidente), "id")]
    public partial class ResidenteFormPageModel : ObservableObject
    {
        private readonly IResidenteRepository _residenteRepository;
        private readonly IAlertaHelper _alertaHelper;

        [ObservableProperty]
        private int idResidente;

        [ObservableProperty]
        private string nombreResidente = string.Empty;

        [ObservableProperty]
        private string apellidoResidente = string.Empty;

        [ObservableProperty]
        private string dniResidente = string.Empty;

        [ObservableProperty]
        private string correoResidente = string.Empty;

        [ObservableProperty]
        private string direccionResidente = string.Empty;

        [ObservableProperty]
        private bool estadoResidente = true;

        [ObservableProperty]
        private bool esEdicion = false;

        partial void OnIdResidenteChanged(int value)
        {
            if (value > 0)
            {
                EsEdicion = true;
                _ = CargarResidenteParaEdicion(value);
            }
            else
            {
                EsEdicion = false;
                LimpiarFormulario(); 
            }
        }
        [RelayCommand]
        public void LimpiarFormulario()
        {
            IdResidente = 0;
            NombreResidente = string.Empty;
            ApellidoResidente = string.Empty;
            DniResidente = string.Empty;
            CorreoResidente = string.Empty;
            DireccionResidente = string.Empty;
            EstadoResidente = true;
            EsEdicion = false;
        }


        public ResidenteFormPageModel(IResidenteRepository residenteRepository,IAlertaHelper alertaHelper)
        {
            _residenteRepository = residenteRepository;
            _alertaHelper = alertaHelper;
        }
        public async Task CargarResidenteParaEdicion(int id)
        {
            try
            {
                var residente = await _residenteRepository.GetResidenteByIdAsync(id);
                if (residente != null)
                {
                    IdResidente = residente.IdResidente;
                    NombreResidente = residente.NombreResidente ?? string.Empty; // Fix for CS8601
                    ApellidoResidente = residente.ApellidoResidente ?? string.Empty; // Fix for CS8601
                    DniResidente = residente.DniResidente ?? string.Empty; // Fix for CS8601
                    CorreoResidente = residente.CorreoResidente ?? string.Empty; // Fix for CS8601
                    DireccionResidente = residente.DireccionResidente ?? string.Empty; // Fix for CS8601
                    EstadoResidente = residente.EstadoResidente;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "El residente no fue encontrado.", "OK");
                    LimpiarFormulario();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar residente para edición: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al cargar el residente: {ex.Message}", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }



        [RelayCommand]
        private async Task CrearResidenteAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreResidente) ||
                string.IsNullOrWhiteSpace(ApellidoResidente) ||
                string.IsNullOrWhiteSpace(DniResidente))
            {
                await Shell.Current.DisplayAlert("Error", "Nombre, Apellido y DNI son campos obligatorios.", "OK");
                return;
            }
            try
            {

                var nuevo = new Residente
                {
                    NombreResidente = NombreResidente,
                    ApellidoResidente = ApellidoResidente,
                    DniResidente = DniResidente,
                    CorreoResidente = CorreoResidente,
                    DireccionResidente = DireccionResidente,
                    EstadoResidente = EstadoResidente,
                    FechaRegistroResidente = DateTime.Now
                };

                await _residenteRepository.CreateResidenteAsync(nuevo);
                await _alertaHelper.ShowSuccessAsync("Ciudadano Registrado Correctamente.");
            }
            catch (Exception ex) // ✅ Captura cualquier excepción durante la creación
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear residente: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al crear el residente: {ex.Message}", "OK");
                
            }
        }


        [RelayCommand]
        private async Task ActualizarResidenteAsync()
        {
            if (IdResidente == 0)
            {
                await Shell.Current.DisplayAlert("Error", "No hay residente para actualizar.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NombreResidente) ||
                string.IsNullOrWhiteSpace(ApellidoResidente) ||
                string.IsNullOrWhiteSpace(DniResidente))
            {
                await Shell.Current.DisplayAlert("Error", "Nombre, Apellido y DNI son campos obligatorios.", "OK");
                return;
            }

            var existente = await _residenteRepository.GetResidenteByIdAsync(IdResidente);
            if (existente == null)
            {
                await Shell.Current.DisplayAlert("Error", "Residente no encontrado.", "OK");
                return;
            }

            var actualizado = new Residente
            {
                IdResidente = IdResidente,
                NombreResidente = NombreResidente,
                ApellidoResidente = ApellidoResidente,
                DniResidente = DniResidente,
                CorreoResidente = CorreoResidente,
                DireccionResidente = DireccionResidente,
                EstadoResidente = EstadoResidente,
                FechaRegistroResidente = existente.FechaRegistroResidente
            };

            await _residenteRepository.UpdateResidenteAsync(actualizado);
            await _alertaHelper.ShowSuccessAsync("Ciudadano Actualizado Correctamente.");
        }
        [RelayCommand]
        private async Task GuardarResidenteAsync()
        {
            if (EsEdicion)
                await ActualizarResidenteAsync();
            else
                await CrearResidenteAsync();
            await Shell.Current.GoToAsync(".."); // Navega hacia atrás

        }

    }
}
