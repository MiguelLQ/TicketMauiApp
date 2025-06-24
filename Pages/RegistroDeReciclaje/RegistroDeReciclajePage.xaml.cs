using MauiFirebase.PageModels.Premios;
using MauiFirebase.PageModels.RegistroDeReciclajePageModel;

namespace MauiFirebase.Pages.RegistroDeReciclaje;

public partial class RegistroDeReciclajePage : ContentPage
{
    private readonly RegistroDeReciclajePageModel _pageModel;

    public RegistroDeReciclajePage(RegistroDeReciclajePageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _pageModel.LoadRegistrosAsync();
    }
}
