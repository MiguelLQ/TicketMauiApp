using System.Text;
using System.Text.Json;
using MauiFirebase.Models;
namespace MauiFirebase.Services;

public class FirebaseRegistroReciclajeService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/registros";

    public async Task<bool> GuardarRegistroFirestoreAsync(RegistroDeReciclaje registro, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}?documentId={id}";

        var body = new
        {
            fields = new
            {
                IdResidente = new { stringValue = registro.IdResidente },
                IdResiduo = new { stringValue = registro.IdResiduo },
                PesoKilogramo = new { doubleValue = Convert.ToDouble(registro.PesoKilogramo) },
                TicketsGanados = new { integerValue = registro.TicketsGanados },
                FechaRegistro = new
                {
                    timestampValue = registro.FechaRegistro.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            }
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[Firestore Error] {response.StatusCode}: {error}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Registro creado correctamente en Firestore");
        }
        return response.IsSuccessStatusCode;
    }


    public async Task<List<RegistroDeReciclaje>> ObtenerRegistrosDesdeFirestoreAsync(string idToken)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.GetAsync(FirestoreBaseUrl);
        if (!response.IsSuccessStatusCode)
        {
            return new List<RegistroDeReciclaje>();
        }

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var lista = new List<RegistroDeReciclaje>();

        if (!doc.RootElement.TryGetProperty("documents", out var documentos))
        {
            return lista;
        }

        foreach (var d in documentos.EnumerateArray())
        {
            var fields = d.GetProperty("fields");

            var registro = new RegistroDeReciclaje
            {
                IDRegistroDeReciclaje = d.GetProperty("name").ToString().Split('/').Last(),
                IdResidente = fields.GetProperty("IdResidente").GetProperty("stringValue").GetString(),
                IdResiduo = fields.GetProperty("IdResiduo").GetProperty("stringValue").GetString(),
                PesoKilogramo = decimal.Parse(fields.GetProperty("PesoKilogramo").GetProperty("doubleValue").GetRawText()),
                TicketsGanados = int.Parse(fields.GetProperty("TicketsGanados").GetProperty("integerValue").GetString() ?? "0"),
                FechaRegistro = DateTime.Parse(fields.GetProperty("FechaRegistro").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                Sincronizado = true
            };

            lista.Add(registro);
        }

        return lista;
    }

    public async Task<List<RegistroDeReciclaje>> ObtenerUltimosRegistrosPorResidenteAsync(string idToken, int residenteId, int cantidad = 10)
    {
        var url = $"https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes/{residenteId}/registroReciclaje?pageSize={cantidad}";
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return new List<RegistroDeReciclaje>();

        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var lista = new List<RegistroDeReciclaje>();

        if (!document.RootElement.TryGetProperty("documents", out var docs))
            return lista;

        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var registro = new RegistroDeReciclaje
            {
                IDRegistroDeReciclaje = doc.GetProperty("name").ToString().Split('/').Last(),
                IdResidente = fields.GetProperty("IdResidente").GetProperty("stringValue").GetString() ?? string.Empty,
                IdResiduo = fields.GetProperty("IdResiduo").GetProperty("stringValue").GetString() ?? string.Empty, 
                PesoKilogramo = decimal.Parse(fields.GetProperty("PesoKilogramo").GetProperty("doubleValue").GetString() ?? "0"),
                FechaRegistro = DateTime.Parse(fields.GetProperty("FechaRegistro").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                TicketsGanados = int.Parse(fields.GetProperty("TicketsGanados").GetProperty("integerValue").GetString() ?? "0")
            };
            lista.Add(registro);
        }

        return lista.OrderByDescending(r => r.FechaRegistro).Take(cantidad).ToList();
    }
}
