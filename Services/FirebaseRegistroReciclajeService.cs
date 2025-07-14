using System.Text;
using System.Text.Json;
using MauiFirebase.Models;
namespace MauiFirebase.Services;

public class FirebaseRegistroReciclajeService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/registroReciclaje";

    public async Task<bool> GuardarRegistroFirestoreAsync(RegistroDeReciclaje registro, string id, string idToken)
    {
        var url = $"https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes/{registro.IdResidente}/registroReciclaje/{id}";

        var body = new
        {
            fields = new
            {
                IdResidente = new { integerValue = registro.IdResidente },
                IdResiduo = new { integerValue = registro.IdResiduo },
                PesoKilogramo = new { doubleValue = registro.PesoKilogramo },
                FechaRegistro = new { timestampValue = registro.FechaRegistro.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") },
                TicketsGanados = new { integerValue = registro.TicketsGanados }
            }
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.PatchAsync(url, content); // Crea o actualiza

        return response.IsSuccessStatusCode;
    }


    public async Task<List<RegistroDeReciclaje>> ObtenerRegistrosDesdeFirestoreAsync(string idToken)
    {
        var url = FirestoreBaseUrl;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<RegistroDeReciclaje>();
        }
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var lista = new List<RegistroDeReciclaje>();
        if (!document.RootElement.TryGetProperty("documents", out var docs))
        {
            return lista;
        }
        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var registro = new RegistroDeReciclaje
            {
                IDRegistroDeReciclaje = doc.GetProperty("name").ToString().Split('/').Last(),
                IdResidente = fields.GetProperty("IdResidente").GetProperty("stringValue").GetString() ?? string.Empty,
                IdResiduo = fields.GetProperty("IdResiduo").GetProperty("integerValue").GetString() ?? string.Empty,
                PesoKilogramo = decimal.Parse(fields.GetProperty("PesoKilogramo").GetProperty("doubleValue").GetString() ?? "0"),
                FechaRegistro = DateTime.Parse(fields.GetProperty("FechaRegistro").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                TicketsGanados = int.Parse(fields.GetProperty("TicketsGanados").GetProperty("integerValue").GetString() ?? "0")
            };
            lista.Add(registro);
        }
        return lista;
    }

    public async Task<List<RegistroDeReciclaje>> ObtenerUltimosRegistrosPorResidenteAsync(string idToken, int residenteId, int cantidad = 10)
    {
        var url = $"https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes/{residenteId}/registroReciclaje?pageSize={cantidad}";
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<RegistroDeReciclaje>();
        }
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var lista = new List<RegistroDeReciclaje>();
        if (!document.RootElement.TryGetProperty("documents", out var docs))
        {

            return lista;
        }
        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var registro = new RegistroDeReciclaje
            {
                IDRegistroDeReciclaje = doc.GetProperty("name").ToString().Split('/').Last(),
                IdResidente = fields.GetProperty("IdResidente").GetProperty("stringValue").GetString() ?? string.Empty,
                IdResiduo = fields.GetProperty("IdResiduo").GetProperty("integerValue").GetString() ?? string.Empty,
                PesoKilogramo = decimal.Parse(fields.GetProperty("PesoKilogramo").GetProperty("doubleValue").GetString() ?? "0"),
                FechaRegistro = DateTime.Parse(fields.GetProperty("FechaRegistro").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                TicketsGanados = int.Parse(fields.GetProperty("TicketsGanados").GetProperty("integerValue").GetString() ?? "0")
            };
            lista.Add(registro);
        }
        // Ordenar por FechaRegistro descendente y tomar los últimos 'cantidad'
        return lista.OrderByDescending(r => r.FechaRegistro).Take(cantidad).ToList();
    }
}
