using System.Text;
using System.Text.Json;
using MauiFirebase.Models;
namespace MauiFirebase.Services;

public class FirebaseConvertidorService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/convertidores";
    public async Task<bool> GuardarConvertidorFirestoreAsync(Convertidor convertidor, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{id}";
        var body = new
        {
            fields = new
            {
                ValorMin = new { integerValue = convertidor.ValorMin },
                ValorMax = new { integerValue = convertidor.ValorMax },
                NumeroTicket = new { integerValue = convertidor.NumeroTicket },
                EstadoConvertidor = new { booleanValue = convertidor.EstadoConvertidor }
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

    public async Task<List<Convertidor>> ObtenerConvertidoresDesdeFirestoreAsync(string idToken)
    {
        var url = FirestoreBaseUrl;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Convertidor>();
        }
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var lista = new List<Convertidor>();
        if (!document.RootElement.TryGetProperty("documents", out var docs))
        {
            return lista;
        }
        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var convertidor = new Convertidor
            {
                IdConvertidor = doc.GetProperty("name").ToString().Split('/').Last(),
                ValorMin = int.Parse(fields.GetProperty("ValorMin").GetProperty("integerValue").GetString() ?? "0"),
                ValorMax = int.Parse(fields.GetProperty("ValorMax").GetProperty("integerValue").GetString() ?? "0"),
                NumeroTicket = int.Parse(fields.GetProperty("NumeroTicket").GetProperty("integerValue").GetString() ?? "0"),
                EstadoConvertidor = fields.GetProperty("EstadoConvertidor").GetProperty("booleanValue").GetBoolean()
            };
            lista.Add(convertidor);
        }
        return lista;
    }

    public async Task<bool> EditarConvertidorFirestoreAsync(Convertidor convertidor, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{id}?updateMask.fieldPaths=ValorMin&updateMask.fieldPaths=ValorMax&updateMask.fieldPaths=NumeroTicket&updateMask.fieldPaths=EstadoConvertidor";

        var body = new
        {
            fields = new
            {
                ValorMin = new { integerValue = convertidor.ValorMin },
                ValorMax = new { integerValue = convertidor.ValorMax },
                NumeroTicket = new { integerValue = convertidor.NumeroTicket },
                EstadoConvertidor = new { booleanValue = convertidor.EstadoConvertidor }
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
}
