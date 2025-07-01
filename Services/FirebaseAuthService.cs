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
        private const string ApiKey = "AIzaSyBLh0YLNn_t2Se1s4jPmZl7wpHjvZp7txQ";
        private const string SignInUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}";
        
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

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var authData = JsonDocument.Parse(result);

                var idToken = authData.RootElement.GetProperty("idToken").GetString();
                var emailReturned = authData.RootElement.GetProperty("email").GetString();
                var refreshToken = authData.RootElement.GetProperty("refreshToken").GetString();
                var uid = authData.RootElement.GetProperty("localId").GetString();

                // Guardar token, refresh token ,email y uid
                Preferences.Set("FirebaseUserId", uid);
                Preferences.Set("FirebaseToken", idToken);
                Preferences.Set("FirebaseRefreshToken", refreshToken);
                Preferences.Set("FirebaseUserEmail", emailReturned);
                // Obtener y guardar el rol
                var rol = await GetUserRoleAsync(uid, idToken);
                Preferences.Set("FirebaseUserRole", rol);
                // Obtener y guardar el correo
                if (!string.IsNullOrWhiteSpace(emailReturned))
                {
                    Preferences.Set("FirebaseUserEmail", emailReturned);
                }

                return true;
            }

            return false;
        }
        public async Task<string> GetUserRoleAsync(string uid, string idToken)
        {
         
            var url = $"https://firestore.googleapis.com/v1/projects/ticketapp-c31cf/databases/(default)/documents/usuarios/{uid}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var rol = doc.RootElement
                    .GetProperty("fields")
                    .GetProperty("rol")
                    .GetProperty("stringValue")
                    .GetString();

                return rol;
            }

            return string.Empty;
        }
      
        public void Logout()
        {
            // ✅ Borrar token local
            Preferences.Remove("FirebaseToken");
            Preferences.Remove("FirebaseUserEmail");
            Preferences.Remove("FirebaseUserId");
            Preferences.Remove("FirebaseRefreshToken");
            Preferences.Remove("FirebaseUserRole");
        }

        public bool IsLoggedIn()
        {
            return Preferences.ContainsKey("FirebaseToken");
        }
        public string GetUserEmail()
        {
            return Preferences.Get("FirebaseUserEmail", string.Empty);
        }
        public string GetIdToken()
        {
            return Preferences.Get("FirebaseToken", string.Empty);
        }

    }
}
