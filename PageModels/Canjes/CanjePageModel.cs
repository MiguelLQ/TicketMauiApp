using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Canjes;

public partial class CanjePageModel : ObservableObject
{
    private readonly ICanjeRepository _canjeRepository;
    private readonly IPremioRepository _premioRepository;

    public ObservableCollection<Canje> ListaCanjes { get; } = new();

    [ObservableProperty] private bool _isBusy;

    public CanjePageModel(ICanjeRepository canjeRepository, IPremioRepository premioRepository)
    {
        _canjeRepository = canjeRepository;
        _premioRepository = premioRepository;
    }

    [RelayCommand]
    public async Task CargarCanjesAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            ListaCanjes.Clear();
            var canjes = await _canjeRepository.GetAllCanjeAync();
            foreach (var canje in canjes)
                ListaCanjes.Add(canje);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CambiarEstadoCanjeAsync(int id)
    {
        await _canjeRepository.ChangeEstadoCanjeAsync(id);
        await CargarCanjesAsync();
    }
}