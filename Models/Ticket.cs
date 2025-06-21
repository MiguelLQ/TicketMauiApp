using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiFirebase.Models
{
        public class Ticket
        {
            [PrimaryKey, AutoIncrement]
            public int IdTicket { get; set; }

            [MaxLength(50)]
            public string? ColorTicket { get; set; }

            public bool EstadoTicket { get; set; }

            public DateTime FechaRegistro { get; set; }
        }
}
