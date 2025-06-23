
using MauiFirebase.PageModels.Residuos;
namespace MauiFirebase.Pages.Residuo;

public partial class ListarResiduoPage : ContentPage
{
	private readonly ResiduoPageModel _pageModel;
	public ListarResiduoPage(ResiduoPageModel pageModel)
	{
		InitializeComponent();
		_pageModel = pageModel;
		BindingContext = _pageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _pageModel.CargarResiduosAsync();
        await _pageModel.CargarCategoriaResiduoAsync(); 
    }
    private async void OnAgregarResiduoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AgregarResiduoPage));
    }

    private async void OnEditarResiduoClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is MauiFirebase.Models.Residuo residuo)
        {
            await Shell.Current.GoToAsync($"{nameof(EditarResiduoPage)}?id={residuo.IdResiduo}");
        }
    }
}