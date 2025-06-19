using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Canjes
{
    public partial class CanjePageModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime _FechaDeCanjeo;
        [ObservableProperty]
        private bool _EstadoCanje = true;

        private Residente? _residenteDniSeleccionada;// residente seleccionada por DNI
        private Premio? _premioSeleccionada;// premio seleccionado por 

        public ObservableCollection<Canje> ListaCanjes { get; } = new();
        public ObservableCollection<Residente> ListaResidentes { get; } = new();
        public ObservableCollection<Premio> ListaPremios { get; } = new();

        private readonly IResiduoRepository _residuoRepository;










     
    }
}
