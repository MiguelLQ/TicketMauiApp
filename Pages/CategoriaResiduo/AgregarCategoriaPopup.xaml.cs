using CommunityToolkit.Maui.Views;
using MauiFirebase.Helpers.Interface;

namespace MauiFirebase.Pages.CategoriaResiduo;

public partial class AgregarCategoriaPopup : Popup,IClosePopup
{
    public AgregarCategoriaPopup()
    {
        InitializeComponent();
    }

    private void CerrarPopupAlTocarFondo(object sender, TappedEventArgs e)
    {
        this.Close();
    }

    private void OnCancelarClicked(object sender, EventArgs e)
    {
        this.Close();
    }
    public void ClosePopup()
    {
        this.Close();
    }
}