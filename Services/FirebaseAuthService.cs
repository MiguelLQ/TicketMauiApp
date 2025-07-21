using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MauiFirebase.Models;
using Microsoft.Maui.Storage;

namespace MauiFirebase.Services
{
    public class FirebaseAuthService
    {
        private const string ApiKey = "AIzaSyD51sCvl0F9s3jJtWQKdkqa8AIbEWGGx9o";
        private const string SignInUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}";
        private const string RefreshUrl = $"https://securetoken.googleapis.com/v1/token?key={ApiKey}";

        public async Task<bool> LoginAsync(string email, string password)
        {
            var client = new HttpClient();

            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(SignInUrl, content);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadAsStringAsync();
            var authData = JsonDocument.Parse(result);

            var idToken = authData.RootElement.GetProperty("idToken").GetString();
            var emailReturned = authData.RootElement.GetProperty("email").GetString();
            var refreshToken = authData.RootElement.GetProperty("refreshToken").GetString();
            var uid = authData.RootElement.GetProperty("localId").GetString();

            // 🔧 Corregido: expiresIn viene como string
            var expiresInStr = authData.RootElement.GetProperty("expiresIn").GetString();
            var expiresInSeconds = int.TryParse(expiresInStr, out var parsed) ? parsed : 3600;

            Preferences.Set("FirebaseUserId", uid);
            Preferences.Set("FirebaseToken", idToken);
            Preferences.Set("FirebaseRefreshToken", refreshToken);
            Preferences.Set("FirebaseUserEmail", emailReturned);
            Preferences.Set("FirebaseTokenExpiry", DateTime.UtcNow.AddSeconds(expiresInSeconds));

            // Rol
            var rol = await GetUserRoleAsync(uid, idToken);
            Preferences.Set("FirebaseUserRole", rol);

            return true;
        }

        public async Task<string> GetUserRoleAsync(string uid, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/usuarios/{uid}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return string.Empty;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("fields", out var fields)) return string.Empty;
            if (!fields.TryGetProperty("rol", out var rolField)) return string.Empty;

            if (rolField.TryGetProperty("stringValue", out var strVal))
                return strVal.GetString() ?? "";
            if (rolField.TryGetProperty("integerValue", out var intVal))
                return intVal.GetInt32().ToString();

            return string.Empty;
        }

        public async Task<string> ObtenerIdTokenSeguroAsync()
        {
            var token = Preferences.Get("FirebaseToken", string.Empty);
            var expiry = Preferences.Get("FirebaseTokenExpiry", DateTime.MinValue);

            if (string.IsNullOrEmpty(token) || expiry <= DateTime.UtcNow)
            {
                token = await RenovarIdTokenAsync();
            }

            return token;
        }

        private async Task<string> RenovarIdTokenAsync()
        {
            var refreshToken = Preferences.Get("FirebaseRefreshToken", string.Empty);
            if (string.IsNullOrEmpty(refreshToken)) return string.Empty;

            var payload = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var content = new FormUrlEncodedContent(payload);
            var client = new HttpClient();
            var response = await client.PostAsync(RefreshUrl, content);

            if (!response.IsSuccessStatusCode) return string.Empty;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var newIdToken = doc.RootElement.GetProperty("id_token").GetString();
            var newRefreshToken = doc.RootElement.GetProperty("refresh_token").GetString();

            var expiresInStr = doc.RootElement.GetProperty("expires_in").GetString();
            var expiresIn = int.TryParse(expiresInStr, out var parsed) ? parsed : 3600;

            Preferences.Set("FirebaseToken", newIdToken);
            Preferences.Set("FirebaseRefreshToken", newRefreshToken);
            Preferences.Set("FirebaseTokenExpiry", DateTime.UtcNow.AddSeconds(expiresIn));

            return newIdToken;
        }
        public async Task<string?> RegistrarAuthUsuarioAsync(string email, string password)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={ApiKey}";

            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var client = new HttpClient();

            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(result);

            return doc.RootElement.GetProperty("localId").GetString(); // el UID del usuario
        }
        // FirebaseAuthService.cs

        public async Task<bool> UsuarioEstaActivoAsync(string uid, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/usuarios/{uid}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return false;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("fields", out var fields)) return false;
            if (!fields.TryGetProperty("estado", out var estadoField)) return false;

            if (estadoField.TryGetProperty("booleanValue", out var estadoVal))
                return estadoVal.GetBoolean(); // true o false

            return false;
        }

        public void Logout()
        {
            Preferences.Remove("FirebaseToken");
            Preferences.Remove("FirebaseUserEmail");
            Preferences.Remove("FirebaseUserId");
            Preferences.Remove("FirebaseRefreshToken");
            Preferences.Remove("FirebaseUserRole");
            Preferences.Remove("FirebaseTokenExpiry");
            Preferences.Remove("FirebaseUserNombre");
            Preferences.Remove("FirebaseUserApellido");

        }

        public bool IsLoggedIn()
        {
            return Preferences.ContainsKey("FirebaseToken") &&
                   Preferences.ContainsKey("FirebaseTokenExpiry") &&
                   Preferences.Get("FirebaseTokenExpiry", DateTime.MinValue) > DateTime.UtcNow;
        }
        public string GetUserId()
        {
            return Preferences.Get("FirebaseUserId", string.Empty);
        }
        public async Task<bool> IntentarRestaurarSesionAsync()
        {
            try
            {
                var userId = Preferences.Get("FirebaseUid", string.Empty);

                if (string.IsNullOrEmpty(userId))
                    return false;

                var token = await ObtenerIdTokenSeguroAsync();

                // Puedes hacer una verificación adicional: llamar a un endpoint protegido de Firebase con el token
                // para asegurarte de que aún es válido (opcional si el token ya se refresca bien).

                return !string.IsNullOrEmpty(token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al restaurar sesión: {ex.Message}");
                return false;
            }
        }



        public string GetUserEmail() => Preferences.Get("FirebaseUserEmail", string.Empty);
        public string GetIdToken() => Preferences.Get("FirebaseToken", string.Empty);
    }
}
