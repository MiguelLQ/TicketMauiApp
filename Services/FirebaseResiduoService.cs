using System.Text;
using System.Text.Json;
using MauiFirebase.Models;
namespace MauiFirebase.Services;

public class FirebaseResiduoService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residuos";
    public async Task<bool> GuardarResiduoFirestoreAsync(Residuo residuo, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{id}";
        var body = new
        {
            fields = new
            {
                IdCategoriaResiduo = new { stringValue = residuo.IdCategoriaResiduo },
                NombreResiduo = new { stringValue = residuo.NombreResiduo },
                EstadoResiduo = new { booleanValue = residuo.EstadoResiduo },
                ValorResiduo = new { integerValue = residuo.ValorResiduo }
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

    public async Task<List<Residuo>> ObtenerResiduosDesdeFirestoreAsync(string idToken)
    {
        var url = FirestoreBaseUrl;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Residuo>();
        }
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var lista = new List<Residuo>();
        if (!document.RootElement.TryGetProperty("documents", out var docs))
        {
            return lista;
        }
        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var residuo = new Residuo
            {
                IdResiduo = doc.GetProperty("name").ToString().Split('/').Last(),
                IdCategoriaResiduo = fields.GetProperty("IdCategoriaResiduo").GetProperty("stringValue").GetString(),
                NombreResiduo = fields.GetProperty("NombreResiduo").GetProperty("stringValue").GetString(),
                EstadoResiduo = fields.GetProperty("EstadoResiduo").GetProperty("booleanValue").GetBoolean(),
                ValorResiduo = int.Parse(fields.GetProperty("ValorResiduo").GetProperty("integerValue").GetString() ?? "0"),
                Sincronizado = true
            };
            lista.Add(residuo);
        }
        return lista;
    }
}