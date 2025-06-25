using MauiFirebase.Models;
using MauiFirebase.PageModels.RegistroDeReciclajes;

namespace MauiFirebase.Pages.RegistroDeReciclaje;

[QueryProperty(nameof(ResidenteSeleccionado), "ResidenteSeleccionado")]
public partial class AgregarRegistroPage : ContentPage
{
    private readonly AgregarRegistroPageModel _viewModel;

    public AgregarRegistroPage(AgregarRegistroPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public Residente ResidenteSeleccionado
    {
        get => _viewModel.ResidenteSeleccionado;
        set => _viewModel.ResidenteSeleccionado = value;
    }
}
