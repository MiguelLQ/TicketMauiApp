using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using MauiFirebase.Pages.RegistroDeReciclaje;

namespace MauiFirebase.PageModels.RegistroDeReciclajes
{
    public class BuscarResidentePageModel : INotifyPropertyChanged
    {
        private readonly IResidenteRepository _residenteRepository;

        private string _dniResidente;
        private readonly IAlertaHelper _alertaHelper;
        public string DniResidente
        {
            get => _dniResidente;
            set { _dniResidente = value; OnPropertyChanged(); }
        }

        public ICommand BuscarResidenteCommand { get; }
        public object ResidenteEncontrado { get; internal set; }

        public BuscarResidentePageModel(IResidenteRepository residenteRepository,
                                        IAlertaHelper alertaHelper)
        {
            _residenteRepository = residenteRepository;
            _alertaHelper = alertaHelper;
            BuscarResidenteCommand = new Command(async () => await BuscarResidenteAsync());
        }

        private async Task BuscarResidenteAsync()
        {
            if (string.IsNullOrWhiteSpace(DniResidente))
            {
                await _alertaHelper.ShowErrorAsync("Ingresa un DNI válido.");
                return;
            }

            var residente = await _residenteRepository.ObtenerPorDniAsync(DniResidente);
            if (residente != null)
            {
                await _alertaHelper.ShowSuccessAsync($"Residente encontrado: {residente.NombreResidente}");

                // Redirigir a AgregarRegistroPage y pasar el objeto residente como parámetro
                var parametros = new Dictionary<string, object>
                {
                    { "ResidenteSeleccionado", residente }
                };
                await Shell.Current.GoToAsync(nameof(AgregarRegistroPage), parametros);
            }
            else
            {
                await AppShell.DisplayToastAsync("Residente no encontrado.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
