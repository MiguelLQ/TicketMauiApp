using MauiFirebase.PageModels.RecuperarContrasenas;

namespace MauiFirebase.Pages.RecuperarContrasena;


public partial class RecuperarContrasenaPage : ContentPage
{
    public RecuperarContrasenaPage()
    {
        InitializeComponent();
        BindingContext = new RecuperarContrasenaPageModel();
    }

}