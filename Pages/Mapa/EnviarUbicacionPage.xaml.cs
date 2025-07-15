using MauiFirebase.PageModels.Mapas;
using Syncfusion.Maui.Toolkit.Carousel;

namespace MauiFirebase.Pages.Mapa;

public partial class EnviarUbicacionPage : ContentPage
{
    private readonly ConductorUbicacionPageModel _viewModel;

    public EnviarUbicacionPage(ConductorUbicacionPageModel viewModel)
	{
		InitializeComponent();
        BindingContext = _viewModel = viewModel;

    }
}