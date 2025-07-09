using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiFirebase.PageModels.CamScaners
{
    public partial class CamScanerPageModel : ObservableObject
    {
        [ObservableProperty]
        private string ?codigoDetectado;

        public async void ProcesarCodigo(string valor)
        {
            CodigoDetectado = valor;

            // Mostrar una alerta con el valor del código
            await Shell.Current.DisplayAlert("Código detectado", valor, "OK");

            // Aquí puedes agregar lógica adicional (guardar en DB, navegar, etc.)
        }
    }
}
