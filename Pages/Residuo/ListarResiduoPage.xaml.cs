using MauiFirebase.Data.Interfaces;
using MauiFirebase.PageModels.Residuos;

namespace MauiFirebase.Pages.Residuo;

public partial class ListarResiduoPage : ContentPage
{
	private readonly ResiduoPageModel _pageModel;
	public ListarResiduoPage(ResiduoPageModel pageModel)
	{
		InitializeComponent();
		_pageModel = pageModel;
		BindingContext = pageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _pageModel.CargarResiduosAsync();
    }

    private async void OnAgregarResiduoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AgregarResiduoPage(_pageModel));
    }

    private async void OnEditarResiduoClicked(object sender, EventArgs e)
    {
       
    }
}