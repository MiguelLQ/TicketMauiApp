using MauiFirebase.PageModels.Residentes;

namespace MauiFirebase.Pages.ResidentesView;

public partial class ResidenteFormPage : ContentPage
{
    private readonly ResidenteFormPageModel _viewModel;
   

    public ResidenteFormPage(ResidenteFormPageModel viewModel)  
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    private async void OnCancelarClicked(object sender, EventArgs e)
    {       
        await Shell.Current.GoToAsync("..");
    }
}

