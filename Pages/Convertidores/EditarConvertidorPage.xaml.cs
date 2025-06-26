using MauiFirebase.PageModels.Conversiones;

namespace MauiFirebase.Pages.Convertidores;

public partial class EditarConvertidorPage : ContentPage
{
	private readonly EditarConvertidorPageModel _viewModel;
    public EditarConvertidorPage(EditarConvertidorPageModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InicializarAsync();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}