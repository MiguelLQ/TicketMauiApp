using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MauiFirebase.Helpers.Interface;

namespace MauiFirebase.Helpers;
public class AlertaHelpers : IAlertaHelper
{
    private readonly SnackbarOptions successOptions = new()
    {
        BackgroundColor = Colors.Green,
        TextColor = Colors.White,
        ActionButtonTextColor = Colors.White,
        CornerRadius = 8,
        Font = Microsoft.Maui.Font.Default
    };

    private readonly SnackbarOptions errorOptions = new()
    {
        BackgroundColor = Colors.Red,
        TextColor = Colors.White,
        ActionButtonTextColor = Colors.White,
        CornerRadius = 8,
        Font = Microsoft.Maui.Font.Default
    };

    public async Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        var options = new SnackbarOptions
        {
            BackgroundColor = Colors.DarkSlateBlue,
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.Yellow,
            CornerRadius = 8,
            Font = Microsoft.Maui.Font.Default
        };

        var snackbar = Snackbar.Make($"{title}: {message}", null, cancel, TimeSpan.FromSeconds(3), options);
        await snackbar.Show();
    }

    public async Task ShowErrorAsync(string message)
    {
        var snackbar = Snackbar.Make($"Error: {message}", null, "Cerrar", TimeSpan.FromSeconds(4), errorOptions);
        await snackbar.Show();
    }

    public async Task ShowSuccessAsync(string message)
    {
        var snackbar = Snackbar.Make($"Éxito: {message}", null, "OK", TimeSpan.FromSeconds(3), successOptions);
        await snackbar.Show();
    }
}
