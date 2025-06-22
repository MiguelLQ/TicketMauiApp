// =================== ResidentesPage.xaml.cs ===================
using MauiFirebase.Data.Interfaces;
using Microsoft.Maui.Controls;
using MauiFirebase.PageModels.Residentes;

namespace MauiFirebase.Pages.ResidentesView;

/// <summary>
/// Pantalla de bienvenida con dos botones:
/// • Registrar nuevo residente
/// • Ver lista de residentes
/// </summary>
public partial class ResidentesPage : ContentPage
{
    public ResidentesPage(ResidentePageModel vm)          // ❶  DI – recibo el VM
    {
        InitializeComponent();
        BindingContext = vm;                              // ❷  lo asigno al BindingContext
    }
}
