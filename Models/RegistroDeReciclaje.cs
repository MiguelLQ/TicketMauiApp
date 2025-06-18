using System;
using SQLite;

namespace MauiFirebase.Models
{
    public class RegistroDeReciclaje
    {
        [PrimaryKey, AutoIncrement]
        public int IDRegistroDeReciclaje { get; set; }

        public int? IdResidente { get; set; } 
        public int? IdResiduo { get; set; }  

        public decimal PesoKilogramo { get; set; }
        public DateTime FechaRegistro { get; set; } 
        public int TicketsGanados { get; set; }
    }
}
