using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Helpers.Interface;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Trabajadores;

public partial class TrabajadorPageModel : ObservableObject
{
    public ObservableCollection<Trabajador> ListaTrabajadores { get; } = new();
    private readonly ITrabajadorRepository _trabajadorRepository;
    private readonly IAlertaHelper _alertaHelper;

    [ObservableProperty]
    private bool isBusy;

    public TrabajadorPageModel(ITrabajadorRepository trabajadorRepository, IAlertaHelper alertaHelper)
    {
        _trabajadorRepository = trabajadorRepository;
        _alertaHelper = alertaHelper;
    }

    [RelayCommand]
    public async Task CargarTrabajadoresAsync()
    {
        try
        {
            IsBusy = true;
            ListaTrabajadores.Clear();
            var trabajadores = await _trabajadorRepository.GetAllTrabajadorAsync();
            foreach (var trabajador in trabajadores)
            {
                ListaTrabajadores.Add(trabajador);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CambiarEstadoTrabajadorAsync(int id)
    {
        await _trabajadorRepository.ChangeEstadoTrabajadorAsync(id);
        await _alertaHelper.ShowSuccessAsync("Se cambió el estado del trabajador correctamente");
        await CargarTrabajadoresAsync();
    }

    [RelayCommand]
    public async Task IrACrearTrabajadorAsync()
    {
        await Shell.Current.GoToAsync("AgregarTrabajadorPage");
    }
}
