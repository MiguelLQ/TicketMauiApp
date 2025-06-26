using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

                // ✅ Guardar el token localmente
                Preferences.Set("FirebaseToken", idToken);

                return true;
            }

            return false;
        }

        public void Logout()
        {
            // ✅ Borrar token local
            Preferences.Remove("FirebaseToken");
        }

        public bool IsLoggedIn()
        {
            return Preferences.ContainsKey("FirebaseToken");
        }
    }
}
