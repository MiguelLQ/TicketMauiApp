using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiFirebase.Services
{
    public class FirebaseUbicacionService
    {
        private readonly HttpClient _httpClient = new();

        private const string BaseUrl = "https://sangeronimomuniapp-default-rtdb.firebaseio.com/";

        public async Task EnviarUbicacionAsync(string uid, object datosUbicacion)
        {
            var url = $"{BaseUrl}ubicacionesVehiculos/{uid}.json";
            var json = JsonSerializer.Serialize(datosUbicacion);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
