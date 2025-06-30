using MauiFirebase.PageModels.Registers;

namespace MauiFirebase.Pages.Register;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = new RegisterPageModel(); // <--- esto es importante
    }
}
