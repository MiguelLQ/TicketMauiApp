// =================== ResidenteListPage.xaml.cs ===================
using Microsoft.Maui.Controls;
using MauiFirebase.PageModels.Residentes;
using MauiFirebase.Models;
namespace MauiFirebase.Pages.ResidentesView;

public partial class ResidenteListPage : ContentPage
{
    private readonly ResidenteListPageModel _vm;
    private bool _yaCargado;

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
        if (!_yaCargado)
        {
            if (BindingContext is ResidenteListPageModel vm)
            {
                await vm.CargarResidentesAsync();
                _yaCargado = true;
            }
        }
    }
    private async void OnBorderTapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement border)
        {
            await border.FadeTo(0.6, 100);
            await border.FadeTo(1.0, 100);
            if (e.Parameter is Residente residente)
            {
                bool confirmar = await Shell.Current.DisplayAlert(
                    "Editar residente",
                    $"¿Deseas editar a {residente.NombreResidente}?",
                    "Sí", 
                    "Cancelar"
                );

                if (confirmar)
                {
                    await Shell.Current.GoToAsync($"residenteForm?id={residente.IdResidente}");
                }                
            }
        }
    }
    private void OnFiltroEstadoChanged(object sender, EventArgs e)
    {
        if (_vm?.AplicarFiltrosCommand?.CanExecute(null) == true)
        {
            _vm.AplicarFiltrosCommand.Execute(null);
        }
    }
}
