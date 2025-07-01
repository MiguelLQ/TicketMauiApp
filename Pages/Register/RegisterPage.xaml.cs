using MauiFirebase.PageModels.Registers;

namespace MauiFirebase.Pages.Register;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetService<RegisterPageModel>();
        // <--- esto es importante
    }
}
