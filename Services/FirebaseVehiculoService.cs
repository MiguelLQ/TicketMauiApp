using System.Text;
using System.Text.Json;
using MauiFirebase.Models;
namespace MauiFirebase.Services;

public class FirebaseVehiculoService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/vehiculos";

    public async Task<bool> GuardarVehiculoFirestoreAsync(Vehiculo vehiculo, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{id}";
        var body = new
        {
            fields = new
            {
                IdUsuario = new { stringValue = vehiculo.IdUsuario },
                PlacaVehiculo = new { stringValue = vehiculo.PlacaVehiculo },
                MarcaVehiculo = new { stringValue = vehiculo.MarcaVehiculo },
                ModeloVehiculo = new { stringValue = vehiculo.ModeloVehiculo },
                EstadoVehiculo = new { booleanValue = vehiculo.EstadoVehiculo },
                FechaRegistroVehiculo = new { timestampValue = vehiculo.FechaRegistroVehiculo.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }
            }
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.PatchAsync(url, content);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Vehiculo>> ObtenerVehiculosDesdeFirestoreAsync(string idToken)
    {
        var url = FirestoreBaseUrl;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Vehiculo>();
        }
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var lista = new List<Vehiculo>();
        if (!document.RootElement.TryGetProperty("documents", out var docs))
        {
            return lista;
        }
        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var vehiculo = new Vehiculo
            {
                IdVehiculo = doc.GetProperty("name").ToString().Split('/').Last(),
                IdUsuario = fields.GetProperty("IdUsuario").GetProperty("stringValue").GetString(),
                PlacaVehiculo = fields.GetProperty("PlacaVehiculo").GetProperty("stringValue").GetString() ?? string.Empty,
                MarcaVehiculo = fields.GetProperty("MarcaVehiculo").GetProperty("stringValue").GetString() ?? string.Empty,
                ModeloVehiculo = fields.GetProperty("ModeloVehiculo").GetProperty("stringValue").GetString() ?? string.Empty,
                EstadoVehiculo = fields.GetProperty("EstadoVehiculo").GetProperty("booleanValue").GetBoolean(),
                FechaRegistroVehiculo = DateTime.Parse(fields.GetProperty("FechaRegistroVehiculo").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString())
            };
            lista.Add(vehiculo);
        }
        return lista;
    }

}
