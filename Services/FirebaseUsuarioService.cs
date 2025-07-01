using System.Net.Http;
using System.Text.Json;
using MauiFirebase.Models;

namespace MauiFirebase.Services
{
    public class FirebaseUsuarioService
    {
        private readonly HttpClient _httpClient = new();

        public async Task<List<Usuario>> ObtenerUsuariosDesdeFirestoreAsync(string idToken)
        {
            var url = "https://firestore.googleapis.com/v1/projects/ticketapp-c31cf/databases/(default)/documents/usuarios";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await _httpClient.GetAsync(url);
            var usuarios = new List<Usuario>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                foreach (var docItem in doc.RootElement.GetProperty("documents").EnumerateArray())
                {
                    var fields = docItem.GetProperty("fields");
                    var nombre = fields.GetProperty("nombre").GetProperty("stringValue").GetString();
                    var correo = fields.GetProperty("email").GetProperty("stringValue").GetString();
                    var rol = fields.GetProperty("rol").GetProperty("stringValue").GetString();
                    var uid = docItem.GetProperty("name").ToString().Split('/').Last();

                    usuarios.Add(new Usuario
                    {
                        Uid = uid,
                        Nombre = nombre,
                        Correo = correo,
                        Rol = rol
                    });
                }
            }

            return usuarios;
        }
    }
}
