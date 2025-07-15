using MauiFirebase.Models;
using System.Text;
using System.Text.Json;

namespace MauiFirebase.Services
{
    public class FirebaseUbicacionService
    {
        private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/ubicacionesVehiculos";
        private const string RealtimeBaseUrl = "https://sangeronimomuniapp-default-rtdb.firebaseio.com/";
        private readonly HttpClient _httpClient;

        public FirebaseUbicacionService()
        {
            _httpClient = new HttpClient(); // ✅ Inicializar HttpClient para reutilizarlo
        }

        // 📍 FIRESTORE (no tocar)
        public async Task<bool> GuardarUbicacionVehiculoFirestoreAsync(UbicacionVehiculo ubicacionVehiculo, string id, string idToken)
        {
            var url = $"{FirestoreBaseUrl}/{id}";

            var body = new
            {
                fields = new
                {
                    IdVehiculo = new { stringValue = ubicacionVehiculo.IdVehiculo ?? "" },
                    Latitud = new { doubleValue = ubicacionVehiculo.Latitud },
                    Longitud = new { doubleValue = ubicacionVehiculo.Longitud },
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await client.PatchAsync(url, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<UbicacionVehiculo>> ObtenerUbicacionesVehiculosDesdeFirestoreAsync(string idToken)
        {
            var url = FirestoreBaseUrl;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new List<UbicacionVehiculo>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(json);
            var lista = new List<UbicacionVehiculo>();

            if (!document.RootElement.TryGetProperty("documents", out var docs))
            {
                return lista;
            }

            foreach (var doc in docs.EnumerateArray())
            {
                var fields = doc.GetProperty("fields");
                var ubicacion = new UbicacionVehiculo
                {
                    IdUbicacionVehiculo = doc.GetProperty("name").ToString().Split('/').Last(),
                    IdVehiculo = fields.GetProperty("IdVehiculo").GetProperty("stringValue").GetString(),
                    Latitud = fields.GetProperty("Latitud").GetProperty("doubleValue").GetDouble(),
                    Longitud = fields.GetProperty("Longitud").GetProperty("doubleValue").GetDouble(),
                };
                lista.Add(ubicacion);
            }
            return lista;
        }

        // Realtime Database (actualizado)
        public async Task EnviarUbicacionAsync(string uid, object datosUbicacion)
        {
            var url = $"{RealtimeBaseUrl}ubicacionesVehiculos/{uid}.json";
            var json = JsonSerializer.Serialize(datosUbicacion);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
        // Obtener todas las ubicaciones de vehículos desde Realtime Database
        public async Task<List<UbicacionVehiculo>> ObtenerTodasUbicacionesAsync()
        {
            var url = $"{RealtimeBaseUrl}ubicacionesVehiculos.json";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<UbicacionVehiculo>();

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json)) return new List<UbicacionVehiculo>();

            var diccionario = JsonSerializer.Deserialize<Dictionary<string, UbicacionVehiculo>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var lista = new List<UbicacionVehiculo>();

            if (diccionario != null)
            {
                foreach (var par in diccionario)
                {
                    var ubicacion = par.Value;
                    if (ubicacion != null)
                    {
                        ubicacion.IdUbicacionVehiculo = par.Key; // Para identificar al UID
                        lista.Add(ubicacion);
                    }
                }
            }

            return lista;
        }

    }
}
