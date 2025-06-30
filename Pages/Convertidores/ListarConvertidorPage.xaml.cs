using MauiFirebase.PageModels.Conversiones;

namespace MauiFirebase.Pages.Convertidores;

public partial class ListarConvertidorPage : ContentPage
{
	private readonly ConversionesPageModel _viewModel;
	public ListarConvertidorPage(ConversionesPageModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarConvertidoresAsync();
    }
    private async void OnAgregarConvertidorClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AgregarConvertidorPage));
    }

    private async void OnEditarConvertidorClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is MauiFirebase.Models.Convertidor convertidor)
        {
            await Shell.Current.GoToAsync($"{nameof(EditarConvertidorPage)}?id={convertidor.IdConvertidor}");
        }
    }
}