using MauiFirebase.PageModels.Logins;

namespace MauiFirebase.Pages.Home;

public partial class InicioPage : ContentPage
{
	public InicioPage()
	{
		InitializeComponent();
        BindingContext = new DashboardPageModel();

    }
}