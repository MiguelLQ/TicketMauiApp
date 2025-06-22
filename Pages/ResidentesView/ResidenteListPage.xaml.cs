// =================== ResidenteListPage.xaml.cs ===================
using Microsoft.Maui.Controls;
using MauiFirebase.PageModels.Residentes;
namespace MauiFirebase.Pages.ResidentesView;

public partial class ResidenteListPage : ContentPage
{
    private readonly ResidenteListPageModel _vm;

    // ➤ Shell usará este
    public ResidenteListPage() : this(
        MauiProgram.Services.GetService<ResidenteListPageModel>()!)
    { }


    public ResidenteListPage(ResidenteListPageModel vm)   // ❶ DI – recibo el VM especializado
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;                             // ❷ BindingContext asignado
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ResidenteListPageModel vm)
            await vm.CargarResidentesAsync();
    }

    // ────────────────────────────────
    // BOTÓN «Agregar»
    // ────────────────────────────────
    private async void OnAgregarResidenteClicked(object sender, EventArgs e)
    {
        // Navega al formulario en modo “nuevo”
        await Shell.Current.GoToAsync("residenteForm");
    }

    // ────────────────────────────────
    // BOTÓN «Editar» (recibe el Id como CommandParameter)
    // ──────────────────────────────── 
    private async void OnEditarResidenteClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is int id)
        {
            await Shell.Current.GoToAsync($"residenteForm?id={id}");
        }
    }
}