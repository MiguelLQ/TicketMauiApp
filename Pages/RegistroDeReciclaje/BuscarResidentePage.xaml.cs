using MauiFirebase.PageModels.RegistroDeReciclajes;
using MauiFirebase.Models;
using System.Text.Json;

namespace MauiFirebase.Pages.RegistroDeReciclaje;

public partial class BuscarResidentePage : ContentPage
{
    private readonly BuscarResidentePageModel _viewModel;

    public BuscarResidentePage(BuscarResidentePageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
