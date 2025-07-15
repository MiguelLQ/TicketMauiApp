using System.Text;
using System.Text.Json;
using MauiFirebase.Models;

namespace MauiFirebase.Services;

public class FirebaseCiudadanoService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/residentes";

    public async Task<bool> GuardarEnFirestoreAsync(Residente residente, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{residente.IdResidente}";

        // Creamos el diccionario de campos manualmente para poder agregar condicionalmente
        var fields = new Dictionary<string, object>
        {
            ["NombreResidente"] = new { stringValue = residente.NombreResidente },
            ["ApellidoResidente"] = new { stringValue = residente.ApellidoResidente },
            ["DniResidente"] = new { stringValue = residente.DniResidente },
            ["CorreoResidente"] = new { stringValue = residente.CorreoResidente },
            ["DireccionResidente"] = new { stringValue = residente.DireccionResidente },
            ["EstadoResidente"] = new { booleanValue = residente.EstadoResidente },
            ["FechaRegistroResidente"] = new { timestampValue = residente.FechaRegistroResidente.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") },
            ["TicketsTotalesGanados"] = new { integerValue = residente.TicketsTotalesGanados }
        };

        // 🔹 Solo si el UidFirebase tiene valor, lo agregamos
        if (!string.IsNullOrWhiteSpace(residente.UidFirebase))
        {
            fields["UidFirebase"] = new { stringValue = residente.UidFirebase };
        }

        var body = new { fields };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.PatchAsync(url, content); // PATCH crea/actualiza usando el ID

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

    public async Task<Residente?> ObtenerResidentePorUidFirebaseAsync(string uid, string idToken)
    {
        var url = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents:runQuery";

        var query = new
        {
            structuredQuery = new
            {
                from = new[] { new { collectionId = "residentes" } },
                where = new
                {
                    fieldFilter = new
                    {
                        field = new { fieldPath = "UidFirebase" },
                        op = "EQUAL",
                        value = new { stringValue = uid }
                    }
                },
                limit = 1
            }
        };

        var json = JsonSerializer.Serialize(query);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await client.PostAsync(url, content);
        if (!response.IsSuccessStatusCode)
            return null;

        var responseJson = await response.Content.ReadAsStringAsync();
        var results = JsonDocument.Parse(responseJson).RootElement;

        foreach (var docResult in results.EnumerateArray())
        {
            if (!docResult.TryGetProperty("document", out var document)) continue;

            var name = document.GetProperty("name").GetString();
            var idResidente = name.Split('/').Last(); // extraer ID del documento
            var fields = document.GetProperty("fields");

            return new Residente
            {
                IdResidente = idResidente,
                NombreResidente = fields.GetProperty("NombreResidente").GetProperty("stringValue").GetString(),
                ApellidoResidente = fields.GetProperty("ApellidoResidente").GetProperty("stringValue").GetString(),
                DniResidente = fields.GetProperty("DniResidente").GetProperty("stringValue").GetString(),
                CorreoResidente = fields.GetProperty("CorreoResidente").GetProperty("stringValue").GetString(),
                DireccionResidente = fields.GetProperty("DireccionResidente").GetProperty("stringValue").GetString(),
                EstadoResidente = fields.GetProperty("EstadoResidente").GetProperty("booleanValue").GetBoolean(),
                FechaRegistroResidente = DateTime.Parse(fields.GetProperty("FechaRegistroResidente").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                TicketsTotalesGanados = int.Parse(fields.GetProperty("TicketsTotalesGanados").GetProperty("integerValue").GetString() ?? "0"),
                UidFirebase = fields.TryGetProperty("UidFirebase", out var uidField) ? uidField.GetProperty("stringValue").GetString() : null
            };
        }

        return null;
    }

}
