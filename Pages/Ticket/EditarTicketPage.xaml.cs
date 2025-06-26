using MauiFirebase.PageModels.Ticket;
using Syncfusion.Maui.Toolkit.Carousel;

namespace MauiFirebase.Pages.Ticket;

public partial class EditarTicketPage : ContentPage
{
    private TicketPageModel _viewModel;

    public EditarTicketPage()
	{
		InitializeComponent();
        // Vinculamos al mismo ViewModel que ya usas (TicketPageModel)
        _viewModel = Shell.Current.CurrentPage?.BindingContext as TicketPageModel;
        BindingContext = _viewModel;

        // Esto reutiliza el contexto ya existente, con sus propiedades y comandos
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("TicketSeleccionado") && _viewModel != null)
        {
            var ticket = query["TicketSeleccionado"] as Models.Ticket;

            if (ticket != null)
                _viewModel.TicketSeleccionado = ticket;
        }
    }
    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(".."); // Regresa a la página anterior
    }
}