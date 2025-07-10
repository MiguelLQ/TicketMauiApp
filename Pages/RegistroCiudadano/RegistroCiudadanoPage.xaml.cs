using MauiFirebase.PageModels.Registers;
using QRCoder;

namespace MauiFirebase.Pages.RegistroCiudadano;

public partial class RegistroCiudadanoPage : ContentPage
{
    private readonly RegistroCiudadanoPageModel _pageModel;

    public RegistroCiudadanoPage(RegistroCiudadanoPageModel pageModel)
	{
		InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }
    public string GenerarQrComoBase64(string texto)
    {
        using var qrGenerator = new QRCoder.QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(texto, QRCoder.QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new QRCoder.PngByteQRCode(qrCodeData);
        var qrBytes = qrCode.GetGraphic(20);

        return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RegistroCiudadanoPageModel vm)
            await vm.InicializarAsync();
    }

    //private void OnGenerateClicked(object sender, EventArgs e)
    //{
    //    QRCodeGenerator qrGenerator = new QRCodeGenerator();
    //    QRCodeData qrCodeData = qrGenerator.CreateQrCode(InputText.Text, QRCodeGenerator.ECCLevel.L);
    //    PngByteQRCode qRCode = new PngByteQRCode(qrCodeData);
    //    byte[] qrCodeBytes = qRCode.GetGraphic(20);
    //    QrCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
    //}
}