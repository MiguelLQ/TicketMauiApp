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
        [NotNull]
        public string? Uid { get; set; }
        [NotNull]
        public string? Nombre { get; set; }
        [NotNull]
        public string? Correo { get; set; }
        [NotNull]
        public string? Rol { get; set; }
        
        public string? Contraseña { get; set; }
    }
}
