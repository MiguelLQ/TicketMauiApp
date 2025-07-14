using System.Text;
using System.Text.Json;
using MauiFirebase.Models;

namespace MauiFirebase.Services;

public class FirebaseCiudadanoService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes";

    public async Task<bool> GuardarEnFirestoreAsync(Residente residente, string idToken)
    {
        var url = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes";

        var body = new
        {
            fields = new
            {
                // No guarda uid, solo los datos del residente
                nombreResidente = new { stringValue = residente.NombreResidente },
                apellidoResidente = new { stringValue = residente.ApellidoResidente },
                dniResidente = new { stringValue = residente.DniResidente },
                correoResidente = new { stringValue = residente.CorreoResidente },
                direccionResidente = new { stringValue = residente.DireccionResidente },
                estadoResidente = new { booleanValue = residente.EstadoResidente },
                fechaRegistroResidente = new { timestampValue = residente.FechaRegistroResidente.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") },
                ticketsTotalesGanados = new { integerValue = residente.TicketsTotalesGanados }
            }
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        // 🔁 POST permite que Firestore genere un ID aleatorio
        var response = await client.PostAsync(url, content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ResidenteExisteEnFirestoreAsync(string uid, string idToken)
    {
        var url = $"https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes/{uid}";

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.GetAsync(url);
        return response.IsSuccessStatusCode;
    }
    public async Task<Residente?> ObtenerResidenteDesdeFirestoreAsync(string uid, string idToken)
    {
        var url = $"https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes/{uid}";

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("fields", out var fields))
            return null;

        return new Residente
        {
            UidResidente = uid,
            NombreResidente = fields.GetProperty("nombreResidente").GetProperty("stringValue").GetString(),
            ApellidoResidente = fields.GetProperty("apellidoResidente").GetProperty("stringValue").GetString(),
            DniResidente = fields.GetProperty("dniResidente").GetProperty("stringValue").GetString(),
            CorreoResidente = fields.GetProperty("correoResidente").GetProperty("stringValue").GetString(),
            DireccionResidente = fields.GetProperty("direccionResidente").GetProperty("stringValue").GetString(),
            EstadoResidente = fields.GetProperty("estadoResidente").GetProperty("booleanValue").GetBoolean(),
            FechaRegistroResidente = DateTime.Parse(fields.GetProperty("fechaRegistroResidente").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
            TicketsTotalesGanados = int.Parse(fields.GetProperty("ticketsTotalesGanados").GetProperty("integerValue").GetString() ?? "0")
        };
    }

}
