using CommunityToolkit.Maui.Views;
using MauiFirebase.Helpers.Interface;

namespace MauiFirebase.Pages.CategoriaResiduo;

public partial class EditarCategoriaPopup : Popup, IClosePopup
{
	public EditarCategoriaPopup()
	{
        InitializeComponent();
	}
    private void OnCancelarClicked(object sender, EventArgs e)
    {
        Close(); // cierra el popup
    }

    private void CerrarPopupAlTocarFondo(object sender, EventArgs e)
    {
        Close(); // también al tocar fuera
    }
    public void ClosePopup()
    {
        Close();
    }
}