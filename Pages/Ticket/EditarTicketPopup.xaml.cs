using CommunityToolkit.Maui.Views;
using MauiFirebase.Helpers.Interface;

namespace MauiFirebase.Pages.Ticket;

public partial class EditarTicketPopup : Popup, IClosePopup
{
    public EditarTicketPopup()
    {
        InitializeComponent();
        
    }

    private void OnCancelarClicked(object sender, EventArgs e)
    {
        Close();
    }

    // Este m�todo cierra el popup si se toca fuera del frame (en el fondo)
    private void CerrarPopupAlTocarFondo(object sender, TappedEventArgs e)
    {
        Close();
    }
    public void ClosePopup()
    {
        this.Close(); // Esto cierra el popup
    }

    // Soluci�n al error CS0121: Aseg�rate de que solo exista una definici�n de InitializeComponent()
    // Si hay m�ltiples definiciones en archivos parciales, verifica que no haya conflictos en el c�digo generado.
    // Si el problema persiste, limpia y reconstruye el proyecto para regenerar los archivos parciales correctamente.
}
