using Microsoft.Maui.Controls;
using MauiFirebase.PageModels.Residentes;

namespace MauiFirebase.Pages.ResidentesView;

public partial class ResidenteFormPage : ContentPage
{
    private readonly ResidenteFormPageModel _vm;
    public ResidenteFormPage() : this(MauiProgram.Services.GetRequiredService<ResidenteFormPageModel>())
    {
    }


    public ResidenteFormPage(ResidenteFormPageModel vm)   // DI
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }
    public string ResidenteId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                _vm.IdResidente = id; // Dispara OnIdResidenteChanged automáticamente
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // ⚠️ Solo limpia si no estamos en modo edición
        if (_vm.IdResidente == 0 && !_vm.EsEdicion)
        {
            _vm.LimpiarFormularioCommand.Execute(null);
        }
    }
}

