using MauiFirebase.PageModels.Premios;
using MauiFirebase.PageModels.RegistroDeReciclajePageModel;

namespace MauiFirebase.Pages.RegistroDeReciclaje;

public partial class CanjePage : ContentPage
{
    private readonly RegistroDeReciclajePageModel _pageModel;

    public CanjePage(RegistroDeReciclajePageModel pageModel)
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
