using System.Text;
using System.Text.Json;
using MauiFirebase.Models;
namespace MauiFirebase.Services;

public class FirebaseCategoriaResiduoService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/categoriasresiduo";
    public async Task<bool> GuardarCategoriaResiduoFirestoreAsync(CategoriaResiduo categoria, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{id}";
        var body = new
        {
            fields = new
            {
                IdTicket = new { stringValue = categoria.IdTicket },
                NombreCategoria = new { stringValue = categoria.NombreCategoria },
                EstadoCategoriaResiduo = new { booleanValue = categoria.EstadoCategoriaResiduo }
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

    public async Task<List<CategoriaResiduo>> ObtenerCategoriasResiduoDesdeFirestoreAsync(string idToken)
    {
        var url = FirestoreBaseUrl;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<CategoriaResiduo>();
        }
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var lista = new List<CategoriaResiduo>();
        if (!document.RootElement.TryGetProperty("documents", out var docs))
        {
            return lista;
        }
        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var categoria = new CategoriaResiduo
            {
                IdCategoriaResiduo = doc.GetProperty("name").ToString().Split('/').Last(),
                IdTicket = fields.GetProperty("IdTicket").GetProperty("stringValue").GetString(),
                NombreCategoria = fields.GetProperty("NombreCategoria").GetProperty("stringValue").GetString(),
                EstadoCategoriaResiduo = fields.GetProperty("EstadoCategoriaResiduo").GetProperty("booleanValue").GetBoolean(),
                Sincronizado = true
            };
            lista.Add(categoria);
        }
        return lista;
    }
}
