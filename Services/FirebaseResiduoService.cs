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
                ValorResiduo = new { doubleValue = Convert.ToDouble(residuo.ValorResiduo) }
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
                IdCategoriaResiduo = ObtenerStringDesdeFirestore(fields, "IdCategoriaResiduo"),
                NombreResiduo = ObtenerStringDesdeFirestore(fields, "NombreResiduo"),
                EstadoResiduo = ObtenerBoolDesdeFirestore(fields, "EstadoResiduo"),
                ValorResiduo = ObtenerDecimalDesdeFirestore(fields, "ValorResiduo"),
                Sincronizado = true
            };
            lista.Add(residuo);
        }
        return lista;
    }

    private string ObtenerStringDesdeFirestore(JsonElement fields, string campo)
    {
        return fields.TryGetProperty(campo, out var valorCampo) &&
               valorCampo.TryGetProperty("stringValue", out var val)
            ? val.GetString() ?? string.Empty
            : string.Empty;
    }

    private bool ObtenerBoolDesdeFirestore(JsonElement fields, string campo)
    {
        return fields.TryGetProperty(campo, out var valorCampo) &&
               valorCampo.TryGetProperty("booleanValue", out var val) &&
               val.ValueKind == JsonValueKind.True;
    }

    private decimal ObtenerDecimalDesdeFirestore(JsonElement fields, string campo)
    {
        if (!fields.TryGetProperty(campo, out var valorCampo))
            return 0;

        foreach (var tipoValor in valorCampo.EnumerateObject())
        {
            if (tipoValor.Value.ValueKind == JsonValueKind.Number)
                return tipoValor.Value.GetDecimal(); 

            if (tipoValor.Value.ValueKind == JsonValueKind.String &&
                decimal.TryParse(tipoValor.Value.GetString(), out var result))
                return result;
        }

        return 0;
    }
}
