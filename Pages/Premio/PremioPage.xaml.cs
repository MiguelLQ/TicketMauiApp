using MauiFirebase.PageModels.Premio;
using Syncfusion.Maui.Toolkit.Carousel;

namespace MauiFirebase.Pages.Premio;

public partial class PremioPage : ContentPage
{

    public PremioPage(PremioPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

    }

}