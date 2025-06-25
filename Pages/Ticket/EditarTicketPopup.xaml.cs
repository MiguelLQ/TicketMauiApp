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

    // Este método cierra el popup si se toca fuera del frame (en el fondo)
    private void CerrarPopupAlTocarFondo(object sender, TappedEventArgs e)
    {
        Close();
    }
    public void ClosePopup()
    {
        this.Close(); // Esto cierra el popup
    }

    // Solución al error CS0121: Asegúrate de que solo exista una definición de InitializeComponent()
    // Si hay múltiples definiciones en archivos parciales, verifica que no haya conflictos en el código generado.
    // Si el problema persiste, limpia y reconstruye el proyecto para regenerar los archivos parciales correctamente.
}
