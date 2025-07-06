using MauiFirebase.PageModels.Trabajadores;
using MauiFirebase.Models;

namespace MauiFirebase.Pages.Trabajador;

public partial class ListarTrabajadorPage : ContentPage
{
    private readonly TrabajadorPageModel _pageModel;
    public ListarTrabajadorPage(TrabajadorPageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _pageModel.CargarTrabajadoresAsync();
    }

    private async void OnAgregarTrabajadorClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("AgregarTrabajadorPage");
    }

    private async void OnEditarTrabajadorClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is MauiFirebase.Models.Trabajador trabajador)
        {
            await Shell.Current.GoToAsync($"EditarTrabajadorPage?id={trabajador.IdTrabajador}");
        }
    }

    private async void OnCambiarEstadoTrabajadorClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is int id)
        {
            await _pageModel.CambiarEstadoTrabajadorAsync(id);
        }
    }
}
