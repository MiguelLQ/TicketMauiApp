using MauiFirebase.Models;
using System.Text;
using System.Text.Json;

namespace MauiFirebase.Services;

public class FirebaseRutaService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/rutas";

    public async Task<bool> GuardarRutaFirestoreAsync(Ruta ruta, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{id}";
        var body = new
        {
            fields = new
            {
                IdVehiculo = new { stringValue = ruta.IdVehiculo ?? string.Empty },
                DiasDeRecoleccion = new { stringValue = ruta.DiasDeRecoleccion ?? string.Empty },
                EstadoRuta = new { booleanValue = ruta.EstadoRuta },
                FechaRegistroRuta = new { timestampValue = ruta.FechaRegistroRuta.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") },
                PuntosRutaJson = new { stringValue = ruta.PuntosRutaJson ?? string.Empty }
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

    public async Task<List<Ruta>> ObtenerRutasDesdeFirestoreAsync(string idToken)
    {
        var url = FirestoreBaseUrl;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Ruta>();
        }
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var lista = new List<Ruta>();
        if (!document.RootElement.TryGetProperty("documents", out var docs))
        {
            return lista;
        }
        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var ruta = new Ruta
            {
                IdRuta = doc.GetProperty("name").ToString().Split('/').Last(),
                IdVehiculo = fields.GetProperty("IdVehiculo").GetProperty("stringValue").GetString(),
                DiasDeRecoleccion = fields.GetProperty("DiasDeRecoleccion").GetProperty("stringValue").GetString(),
                EstadoRuta = fields.GetProperty("EstadoRuta").GetProperty("booleanValue").GetBoolean(),
                FechaRegistroRuta = DateTime.Parse(fields.GetProperty("FechaRegistroRuta").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                PuntosRutaJson = fields.GetProperty("PuntosRutaJson").GetProperty("stringValue").GetString(),
                Sincronizado = true
            };
            lista.Add(ruta);
        }
        return lista;
    }

}
