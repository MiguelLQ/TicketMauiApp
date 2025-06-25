using CommunityToolkit.Maui.Views;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Repositories;
using MauiFirebase.PageModels.Ticket;
using SQLite;

namespace MauiFirebase.Pages.Ticket;

public partial class ListarTicketPage : ContentPage
{
    private readonly TicketPageModel _pageModel;

    public ListarTicketPage(TicketPageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }



    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _pageModel.LoadTicketsAsync();
    }
    private async void OnEditarTicketClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var ticket = button?.BindingContext as Models.Ticket;
        var viewModel = BindingContext as TicketPageModel;

        if (ticket != null && viewModel != null)
        {
            viewModel.TicketSeleccionado = ticket;

            var popup = new EditarTicketPopup(viewModel);
            await this.ShowPopupAsync(popup);
        }
    }
}