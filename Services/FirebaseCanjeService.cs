using System.Text;
using System.Text.Json;
using MauiFirebase.Models;

namespace MauiFirebase.Services;

public class FirebaseCanjeService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/canjes";

    public async Task<bool> GuardarCanjeFirestoreAsync(Canje canje, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{id}";

        var body = new
        {
            fields = new
            {
                IdCanje = new { stringValue = id },
                IdResidente = new { stringValue = canje.IdResidente ?? string.Empty },
                IdPremio = new { stringValue = canje.IdPremio ?? string.Empty },
                FechaCanje = new { timestampValue = canje.FechaCanje.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                EstadoCanje = new { booleanValue = canje.EstadoCanje }
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

    public async Task<List<Canje>> ObtenerCanjesDesdeFirestoreAsync(string idToken)
    {
        var lista = new List<Canje>();

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.GetAsync(FirestoreBaseUrl);
        if (!response.IsSuccessStatusCode)
            return lista;

        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("documents", out var docs))
            return lista;

        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");

            var canje = new Canje
            {
                IdCanje = fields.GetProperty("IdCanje").GetProperty("stringValue").GetString() ?? string.Empty,
                IdResidente = fields.GetProperty("IdResidente").GetProperty("stringValue").GetString() ?? string.Empty,
                IdPremio = fields.GetProperty("IdPremio").GetProperty("stringValue").GetString() ?? string.Empty,
                FechaCanje = DateTime.Parse(fields.GetProperty("FechaCanje").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                EstadoCanje = fields.GetProperty("EstadoCanje").GetProperty("booleanValue").GetBoolean(),
                Sincronizado = true
            };

            lista.Add(canje);
        }

        return lista;
    }
}
