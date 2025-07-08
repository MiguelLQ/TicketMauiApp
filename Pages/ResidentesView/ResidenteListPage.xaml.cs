using MauiFirebase.PageModels.Residentes;

namespace MauiFirebase.Pages.ResidentesView;

public partial class ResidenteListPage : ContentPage
{
    private readonly ResidenteListPageModel _viewModel;

    public ResidenteListPage(ResidenteListPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarResidentesAsync();
    }

    private async void OnAgregarResidenteClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ResidenteFormPage));
    }
}
