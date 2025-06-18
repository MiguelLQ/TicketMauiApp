using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiFirebase.Models
{
    public class Residente
    {

        public string? IdResidente { get; set; }
        public string? IdUsuario { get; set; }
        public string? NombreResidente { get; set; }
        public string? ApellidoResidente { get; set; }
        public string? DniResidente { get; set; }
        public string? CorreoResidente { get; set; }
        public string? DireccionResidente { get; set; }
        public int? EstadoResidente { get; set; }
        public DateTime FechaRegistroResidente { get; set; } = DateTime.Now;
        public int TicketsTotalesGanados { get; set; } = 0;
        
    }
}
