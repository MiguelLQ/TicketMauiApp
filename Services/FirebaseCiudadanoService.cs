using System.Text;
using System.Text.Json;
using MauiFirebase.Models;

namespace MauiFirebase.Services;

public class FirebaseCiudadanoService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes";

    public async Task<bool> GuardarEnFirestoreAsync(Residente residente, string idToken)
    {
        // 👇 Aquí usamos el UidResidente como ID del documento
        var url = $"{FirestoreBaseUrl}/{residente.IdResidente}";

        var body = new
        {
            fields = new
            {
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

        var response = await client.PatchAsync(url, content); // PATCH usa el ID que pusiste

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ResidenteExisteEnFirestoreAsync(string idResidente, string idToken)
    {
        var url = $"https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes/{idResidente}";

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.GetAsync(url);
        return response.IsSuccessStatusCode;
    }

    public async Task<Residente?> ObtenerResidenteDesdeFirestoreAsync(string idResidente, string idToken)
    {
        var url = $"https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes/{idResidente}";

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
            IdResidente = idResidente, //asignarl el id
            NombreResidente = fields.GetProperty("NombreResidente").GetProperty("stringValue").GetString(),
            ApellidoResidente = fields.GetProperty("ApellidoResidente").GetProperty("stringValue").GetString(),
            DniResidente = fields.GetProperty("DniResidente").GetProperty("stringValue").GetString(),
            CorreoResidente = fields.GetProperty("CorreoResidente").GetProperty("stringValue").GetString(),
            DireccionResidente = fields.GetProperty("DireccionResidente").GetProperty("stringValue").GetString(),
            EstadoResidente = fields.GetProperty("EstadoResidente").GetProperty("booleanValue").GetBoolean(),
            FechaRegistroResidente = DateTime.Parse(fields.GetProperty("FechaRegistroResidente").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
            TicketsTotalesGanados = int.Parse(fields.GetProperty("TicketsTotalesGanados").GetProperty("integerValue").GetString() ?? "0")
        };
    }



}
