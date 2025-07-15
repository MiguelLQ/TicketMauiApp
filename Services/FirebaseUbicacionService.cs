using System.Net.Http;
using MauiFirebase.Models;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiFirebase.Services;

namespace MauiFirebase.Services
{
public class FirebaseUbicacionService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/ubicacionesVehiculos";
        private readonly HttpClient _httpClient = new();

    public async Task<bool> GuardarUbicacionVehiculoFirestoreAsync(UbicacionVehiculo ubicacionVehiculo, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{id}";
        private const string BaseUrl = "https://sangeronimomuniapp-default-rtdb.firebaseio.com/";

        public async Task EnviarUbicacionAsync(string uid, object datosUbicacion)
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
            var url = $"{BaseUrl}ubicacionesVehiculos/{uid}.json";
            var json = JsonSerializer.Serialize(datosUbicacion);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.PatchAsync(url, content);
        return response.IsSuccessStatusCode;
            var response = await _httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();
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
}
