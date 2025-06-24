using MauiFirebase.Helpers.Interface;

namespace MauiFirebase.Helpers;
public class AlertaHelpers : IAlertaHelper
{
    public async Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (mainPage != null)
        {
            await mainPage.DisplayAlert(title, message, cancel);
        }
        else
        {
            throw new InvalidOperationException("No se pudo encontrar la página principal.");
        }
    }

    public async Task ShowErrorAsync(string message)
    {
        await ShowAlertAsync("Error", message);
    }

    public async Task ShowSuccessAsync(string message)
    {
        await ShowAlertAsync("Éxito", message);
    }

}
