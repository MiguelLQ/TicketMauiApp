using System.Text;
using System.Text.Json;
using MauiFirebase.Models;
namespace MauiFirebase.Services;

public class FirebaseResidenteService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes";

    public async Task<bool> GuardarResidenteFirestoreAsync(Residente residente, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{id}";
        var body = new
        {
            fields = new
            {
                NombreResidente = new { stringValue = residente.NombreResidente },
                ApellidoResidente = new { stringValue = residente.ApellidoResidente },
                DniResidente = new { stringValue = residente.DniResidente },
                CorreoResidente = new { stringValue = residente.CorreoResidente },
                DireccionResidente = new { stringValue = residente.DireccionResidente },
                EstadoResidente = new { booleanValue = residente.EstadoResidente },
                FechaRegistroResidente = new { timestampValue = residente.FechaRegistroResidente.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") },
                TicketsTotalesGanados = new { integerValue = residente.TicketsTotalesGanados },
                UidFirebase = new { stringValue = residente.UidFirebase ?? "" }
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

    public async Task<List<Residente>> ObtenerResidentesDesdeFirestoreAsync(string idToken)
    {
        var url = FirestoreBaseUrl;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Residente>();
        }
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var lista = new List<Residente>();
        if (!document.RootElement.TryGetProperty("documents", out var docs))
        {
            return lista;
        }
        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var residente = new Residente
            {
                IdResidente = doc.GetProperty("name").ToString().Split('/').Last(),
                NombreResidente = fields.GetProperty("NombreResidente").GetProperty("stringValue").GetString(),
                ApellidoResidente = fields.GetProperty("ApellidoResidente").GetProperty("stringValue").GetString(),
                DniResidente = fields.GetProperty("DniResidente").GetProperty("stringValue").GetString(),
                CorreoResidente = fields.GetProperty("CorreoResidente").GetProperty("stringValue").GetString(),
                DireccionResidente = fields.GetProperty("DireccionResidente").GetProperty("stringValue").GetString(),
                EstadoResidente = fields.GetProperty("EstadoResidente").GetProperty("booleanValue").GetBoolean(),
                FechaRegistroResidente = DateTime.Parse(fields.GetProperty("FechaRegistroResidente").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                TicketsTotalesGanados = int.Parse(fields.GetProperty("TicketsTotalesGanados").GetProperty("integerValue").GetString() ?? "0"),
                UidFirebase = fields.TryGetProperty("UidFirebase", out var uidProp)
                        ? uidProp.GetProperty("stringValue").GetString()
                        : string.Empty,
                Sincronizado = true
            };
            lista.Add(residente);
        }
        return lista;
    }

}
