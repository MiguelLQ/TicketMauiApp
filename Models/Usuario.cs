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
        public string? Apellido { get; set; }= string.Empty;
        [NotNull]
        public string? Correo { get; set; }
        [NotNull]
        public string? Rol { get; set; }
        public string? Contraseña { get; set; }
        public string? FotoUrl { get; set; } // URL pública de Supabase
        public string? FotoLocal { get; set; }
        // 👇 Esta propiedad se usará en la UI
        public ImageSource FotoSource =>
            !string.IsNullOrWhiteSpace(FotoLocal) && File.Exists(FotoLocal)
        ? ImageSource.FromFile(FotoLocal)
        : (!string.IsNullOrWhiteSpace(FotoUrl)
            ? ImageSource.FromUri(new Uri(FotoUrl))
            : ImageSource.FromFile("userlogo.png")); // Imagen por defecto


        [NotNull]
        public string? Telefono { get; set; }
        [NotNull]
        public bool Estado { get; set; } = true;
    }
}
