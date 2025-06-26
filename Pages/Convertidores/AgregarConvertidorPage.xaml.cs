using MauiFirebase.PageModels.Conversiones;

namespace MauiFirebase.Pages.Convertidores;

public partial class AgregarConvertidorPage : ContentPage
{
	private readonly CrearConvertidorPageModel _viewModel;
	public AgregarConvertidorPage(CrearConvertidorPageModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarConvertidorAsync();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}