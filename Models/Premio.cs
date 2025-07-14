using SQLite;

namespace MauiFirebase.Models
{
    public class Premio
    {

        [PrimaryKey, AutoIncrement]
        public int IdPremio { get; set; }

        public string NombrePremio { get; set; } = string.Empty;
        public string DescripcionPremio { get; set; } = string.Empty;

        public int PuntosRequeridos { get; set; }

        public bool EstadoPremio { get; set; }
        // 📷 URL pública de Supabase
        public string FotoPremioUrl{ get; set; } = string.Empty;

        public string FotoPremio { get; set; } = string.Empty;

    }
}