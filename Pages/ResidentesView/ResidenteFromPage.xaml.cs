using Microsoft.Maui.Controls;
using MauiFirebase.PageModels.Residentes;

namespace MauiFirebase.Pages.ResidentesView;

public partial class ResidenteFormPage : ContentPage
{
    private readonly ResidenteFormPageModel _vm;

    public ResidenteFormPage(ResidenteFormPageModel vm)   // DI
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.IdResidente == 0)
            _vm.LimpiarFormularioCommand.Execute(null);
    }
}

