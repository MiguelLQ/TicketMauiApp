using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        public async Task<string?> AgregarUsuarioAsync(Usuario usuario, string idToken)
        {
            var url = "https://firestore.googleapis.com/v1/projects/ticketapp-c31cf/databases/(default)/documents/usuarios";

            var payload = new
            {
                fields = new
                {
                    nombre = new { stringValue = usuario.Nombre },
                    email = new { stringValue = usuario.Correo },
                    rol = new { stringValue = usuario.Rol }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", idToken);

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseBody = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(responseBody);
            var name = document.RootElement.GetProperty("name").GetString();

            // name = "projects/xxx/databases/(default)/documents/usuarios/ABC123"
            var uid = name?.Split('/').Last();

            return uid;
        }


    }
}
