using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Logins
{
    public class TarjetaResumen
    {
        public string Titulo { get; set; }
        public string Valor { get; set; }
        public string Icono { get; set; } // Usa imágenes o emojis
    }

    public class DashboardPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TarjetaResumen> TarjetasResumen { get; set; }

        public DashboardPageModel()
        {
            // Datos simulados
            TarjetasResumen = new ObservableCollection<TarjetaResumen>
            {
                new TarjetaResumen { Titulo = "Residentes", Valor = "25", Icono = "dotnet_bot.png" },
                new TarjetaResumen { Titulo = "Reciclaje", Valor = "234.5 kg", Icono = "dotnet_bot.png" },
                new TarjetaResumen { Titulo = "Tickets", Valor = "19", Icono = "dotnet_bot.png" },
                new TarjetaResumen { Titulo = "Premios", Valor = "7", Icono = "dotnet_bot.png" },
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
