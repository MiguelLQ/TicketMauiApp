using MauiFirebase.PageModels.Premios;

namespace MauiFirebase.Pages.Premio;

public partial class ListarPremioPage : ContentPage
{
    private readonly PremioPageModel _viewModel;

    public ListarPremioPage(PremioPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.VerificarRol();

        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            //Sincroniza Firestore -> SQLite
            await _viewModel.SincronizarPremiosDesdeFirestoreAsync();
        }

        //Carga siempre desde SQLite
        await _viewModel.CargarPremiosAsync();
    }



}

