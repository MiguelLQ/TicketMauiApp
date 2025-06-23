namespace MauiFirebase.Helpers.Interface;
public interface IAlertaHelper
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task ShowSuccessAsync(string message);
    Task ShowErrorAsync(string message);
}
