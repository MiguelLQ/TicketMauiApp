using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Canjes;

[QueryProperty(nameof(IdCanje), "id")]
public partial class EditarCanjePageModel : ObservableObject
{
    private readonly ICanjeRepository _canjeRepository;
    private readonly IPremioRepository _premioRepository;
    private readonly IAlertaHelper _alertaHelper;

    public ObservableCollection<Premio> ListaPremios { get; } = new();

    [ObservableProperty] 
    private string? _idCanje;
    [ObservableProperty] 
    private Premio? _premioSeleccionado;
    [ObservableProperty] 
    private bool _estadoCanje;
    [ObservableProperty] 
    private Canje? _canjeSeleccionado;

    public EditarCanjePageModel(
        ICanjeRepository canjeRepository,
        IPremioRepository premioRepository,
        IAlertaHelper alertaHelper)
    {
        _canjeRepository = canjeRepository;
        _premioRepository = premioRepository;
        _alertaHelper = alertaHelper;
    }

    public async Task InicializarAsync()
    {
        await CargarPremiosAsync();
        CanjeSeleccionado = await _canjeRepository.GetCanjeIdAsync(IdCanje!);

        if (CanjeSeleccionado != null)
        {
            EstadoCanje = CanjeSeleccionado.EstadoCanje;
            PremioSeleccionado = ListaPremios.FirstOrDefault(p => p.IdPremio == CanjeSeleccionado.IdPremio);
        }
    }

    [RelayCommand]
    public async Task CargarPremiosAsync()
    {
        ListaPremios.Clear();
        var premios = await _premioRepository.GetAllPremiosAsync();
        foreach (var p in premios)
        {
            ListaPremios.Add(p);
        }
    }

    [RelayCommand]
    public async Task GuardarCambiosAsync()
    {
        if (CanjeSeleccionado == null)
        {
            return;
        }

        CanjeSeleccionado.EstadoCanje = EstadoCanje;
        CanjeSeleccionado.IdPremio = PremioSeleccionado?.IdPremio ?? string.Empty;

        await _canjeRepository.UpdateCanjeAsync(CanjeSeleccionado);
        await _alertaHelper.ShowSuccessAsync("Canje actualizado correctamente.");
        await Shell.Current.GoToAsync("..");
    }
}
