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
        private const string ApiKey = "AIzaSyBLh0YLNn_t2Se1s4jPmZl7wpHjvZp7txQ";
        private const string projectId = "ticketapp-c31cf";


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
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var firestoreData = new
            {
                fields = new
                {
                    nombre = new { stringValue = usuario.Nombre },
                    correo = new { stringValue = usuario.Correo },
                    rol = new { stringValue = usuario.Rol }
                }
            };

            var json = JsonSerializer.Serialize(firestoreData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // ⚠️ Aquí está la clave: usar el UID como ID del documento
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/usuarios/{usuario.Uid}";

            var response = await client.PatchAsync(url, content); // PATCH: si existe actualiza, si no, crea
            return response.IsSuccessStatusCode ? usuario.Uid : null;
        }


        public async Task<string?> RefreshIdTokenAsync()
        {
            var refreshToken = Preferences.Get("FirebaseRefreshToken", string.Empty);
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var payload = new
            {
                grant_type = "refresh_token",
                refresh_token = refreshToken
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var response = await client.PostAsync($"https://securetoken.googleapis.com/v1/token?key={ApiKey}", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(result);

            var newIdToken = doc.RootElement.GetProperty("id_token").GetString();
            var newRefreshToken = doc.RootElement.GetProperty("refresh_token").GetString();

            // Guardar los nuevos tokens
            Preferences.Set("FirebaseToken", newIdToken);
            Preferences.Set("FirebaseRefreshToken", newRefreshToken);

            return newIdToken;
        }

    }
}
