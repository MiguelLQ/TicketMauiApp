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
}