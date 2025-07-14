using System.Text;
using System.Text.Json;
using MauiFirebase.Models;
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
                IdResidente = new { integerValue = canje.IdResidente },
                IdPremio = new { integerValue = canje.IdPremio },
                FechaCanje = new { timestampValue = canje.FechaCanje.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") },
                EstadoCanje = new { booleanValue = canje.EstadoCanje }
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

    public async Task<List<Canje>> ObtenerCanjesPorResidenteDesdeFirestoreAsync(string idToken, int residenteId)
    {
        var url = $"{FirestoreBaseUrl}/{residenteId}/canjes";
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Canje>();
        }
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
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
                IdCanje = doc.GetProperty("name").ToString().Split('/').Last(),
                IdResidente = fields.GetProperty("IdResidente").GetProperty("stringValue").GetString() ?? string.Empty,
                IdPremio = fields.GetProperty("IdPremio").GetProperty("stringValue").GetString() ?? string.Empty,
                FechaCanje = DateTime.Parse(fields.GetProperty("FechaCanje").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                EstadoCanje = fields.GetProperty("EstadoCanje").GetProperty("booleanValue").GetBoolean()
            };
            lista.Add(canje);
        }
        return lista;
    }
}
