using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiFirebase.Models
{
    public class Usuario
    {
        [PrimaryKey]
        public string? Uid { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Rol { get; set; }
    }
}
