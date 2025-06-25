using MauiFirebase.PageModels.Canjes;

namespace MauiFirebase.Pages.Canje;

public partial class ListarCanjePage : ContentPage
{
    private readonly CanjePageModel _pageModel;

    public ListarCanjePage(CanjePageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _pageModel.CargarCanjesAsync();
    }

    private async void OnAgregarCanjeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AgregarCanjePage));
    }

    private async void OnEditarCanjeClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is MauiFirebase.Models.Canje canje)
        {
            await Shell.Current.GoToAsync($"{nameof(EditarCanjePage)}?id={canje.IdCanje}");
        }
    }
}
