// =================== ResidenteListPage.xaml.cs ===================

using Microsoft.Maui.Controls;
using MauiFirebase.PageModels.Residentes;
using MauiFirebase.Models;


namespace MauiFirebase.Pages.ResidentesView;

public partial class ResidenteListPage : ContentPage
{
    private readonly ResidenteListPageModel _vm;
    private bool _yaCargado;
    private void OnNombreTextChanged(object sender, TextChangedEventArgs e)
    {
        _vm?.AplicarFiltrosCommand?.Execute(null);
    }


    public ResidenteListPage(ResidenteListPageModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.CargarResidentesAsync();
    }

    // Evento al hacer tap sobre un contenedor de residente
    private async void OnBorderTapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement border)
        {
            await border.FadeTo(0.6, 100);  // Animación visual
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

    // Se ejecuta cuando se cambia el filtro de estado en el Picker
    private void OnFiltroEstadoChanged(object sender, EventArgs e)
    {
        if (_vm?.AplicarFiltrosCommand?.CanExecute(null) == true)
        {
            _vm.AplicarFiltrosCommand.Execute(null);
        }
    }
}
