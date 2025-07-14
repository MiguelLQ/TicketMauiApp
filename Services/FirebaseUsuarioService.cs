using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MauiFirebase.Models;
namespace MauiFirebase.Services;

public class FirebaseUsuarioService
{
    private readonly HttpClient _httpClient = new();
    private const string ApiKey = "AIzaSyD51sCvl0F9s3jJtWQKdkqa8AIbEWGGx9o";
    private const string projectId = "sangeronimomuniapp";


    public async Task<List<Usuario>> ObtenerUsuariosDesdeFirestoreAsync(string idToken)
    {
        var url = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/usuarios";
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

                string uid = docItem.GetProperty("name").ToString().Split('/').Last();

                string nombre = fields.GetProperty("nombre").GetProperty("stringValue").GetString() ?? "";
                string apellido = fields.TryGetProperty("apellido", out var ap) ? ap.GetProperty("stringValue").GetString() ?? "" : "";
                string correo = fields.TryGetProperty("correo", out var co) ? co.GetProperty("stringValue").GetString() ?? "" : "";
                string telefono = fields.TryGetProperty("telefono", out var tel) ? tel.GetProperty("stringValue").GetString() ?? "" : "";
                string rol = fields.TryGetProperty("rol", out var rl) ? rl.GetProperty("stringValue").GetString() ?? "" : "";
                string foto = fields.TryGetProperty("foto", out var ft) ? ft.GetProperty("stringValue").GetString() ?? "" : "";
                bool estado = fields.TryGetProperty("estado", out var es) && es.TryGetProperty("booleanValue", out var bval) && bval.GetBoolean();

                usuarios.Add(new Usuario
                {
                    Uid = uid,
                    Nombre = nombre,
                    Apellido = apellido,
                    Correo = correo,
                    Telefono = telefono,
                    Rol = rol,
                    FotoLocal = foto,
                    Estado = estado
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
                apellido = new { stringValue = usuario.Apellido },
                correo = new { stringValue = usuario.Correo },
                rol = new { stringValue = usuario.Rol },
                telefono = new { stringValue = usuario.Telefono },
                estado = new { booleanValue = usuario.Estado },
                //foto = new { stringValue = usuario.Foto ?? "" }
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
    public async Task<bool> EditarUsuarioEnFirestoreAsync(Usuario usuario, string idToken)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", idToken);

        var firestoreData = new
        {
            fields = new
            {
                rol = new { stringValue = usuario.Rol },
                estado = new { booleanValue = usuario.Estado }
            }
        };

        var json = JsonSerializer.Serialize(firestoreData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // URL con UID del usuario
        var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/usuarios/{usuario.Uid}?updateMask.fieldPaths=rol&updateMask.fieldPaths=estado";

        var response = await client.PatchAsync(url, content);
        return response.IsSuccessStatusCode;
    }
    public async Task<Usuario?> ObtenerUsuarioPorUidAsync(string uid, string idToken)
    {
        var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/usuarios/{uid}";

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", idToken);

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("fields", out var fields))
            return null;

        string nombre = fields.GetProperty("nombre").GetProperty("stringValue").GetString() ?? "";
        string apellido = fields.TryGetProperty("apellido", out var ap) ? ap.GetProperty("stringValue").GetString() ?? "" : "";
        string correo = fields.TryGetProperty("correo", out var co) ? co.GetProperty("stringValue").GetString() ?? "" : "";
        string telefono = fields.TryGetProperty("telefono", out var tel) ? tel.GetProperty("stringValue").GetString() ?? "" : "";
        string rol = fields.TryGetProperty("rol", out var rl) ? rl.GetProperty("stringValue").GetString() ?? "" : "";
        string foto = fields.TryGetProperty("foto", out var ft) ? ft.GetProperty("stringValue").GetString() ?? "" : "";
        bool estado = fields.TryGetProperty("estado", out var es) && es.TryGetProperty("booleanValue", out var bval) && bval.GetBoolean();

        return new Usuario
        {
            Uid = uid,
            Nombre = nombre,
            Apellido = apellido,
            Correo = correo,
            Telefono = telefono,
            Rol = rol,
            FotoLocal = foto,
            Estado = estado
        };
    }
}
