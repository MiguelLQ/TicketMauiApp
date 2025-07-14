using MauiFirebase.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
namespace MauiFirebase.Services;

public class FirebaseCanjeService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes";

    public async Task<bool> GuardarCanjeFirestoreAsync(Canje canje, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{canje.IdResidente}/canjes/{id}";
        var body = new
        {
            fields = new
            {
                IdCanje = new { stringValue = id },
                IdResidente = new { integerValue = canje.IdResidente },
                IdPremio = new { integerValue = canje.IdPremio },
                FechaCanje = new { timestampValue = canje.FechaCanje.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                EstadoCanje = new { booleanValue = canje.EstadoCanje }
            }
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.PatchAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            Debug.WriteLine($"Error guardando canje: {response.StatusCode}, {responseBody}");
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Canje>> ObtenerCanjesPorResidenteDesdeFirestoreAsync(string idToken, string residenteId)
    {
        var url = $"{FirestoreBaseUrl}/{residenteId}/canjes";
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            Debug.WriteLine($"Error obteniendo canjes: {response.StatusCode}, {responseBody}");
            return new List<Canje>();
        }

        var document = JsonDocument.Parse(responseBody);
        var lista = new List<Canje>();
        if (!document.RootElement.TryGetProperty("documents", out var docs))
        {
            return lista;
        }
        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var canje = new Canje
            {
                IdCanje = fields.GetProperty("IdCanje").GetProperty("stringValue").GetString() ?? string.Empty,
                IdResidente = fields.GetProperty("IdResidente").GetProperty("integerValue").GetInt32().ToString(),
                IdPremio = fields.GetProperty("IdPremio").GetProperty("integerValue").GetInt32().ToString(),
                FechaCanje = DateTime.Parse(fields.GetProperty("FechaCanje").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                EstadoCanje = fields.GetProperty("EstadoCanje").GetProperty("booleanValue").GetBoolean()
            };
            lista.Add(canje);
        }
        return lista;
    }
}
