using CommunityToolkit.Maui.Views;
using MauiFirebase.Helpers.Interface;

namespace MauiFirebase.Pages.Ticket;

public partial class AgregarTicketPopup : Popup, IClosePopup
{
    public AgregarTicketPopup()
    {
        InitializeComponent();
    }

    private void OnCancelarClicked(object sender, EventArgs e)
    {
        Close();
    }

    private void CerrarPopupAlTocarFondo(object sender, TappedEventArgs e)
    {
        Close();
    }

    public void ClosePopup()
    {
        this.Close(); //esto cierra el popup  
    }
}
