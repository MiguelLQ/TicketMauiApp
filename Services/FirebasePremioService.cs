using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MauiFirebase.Models;

namespace MauiFirebase.Services
{
    public class FirebasePremioService
    {
        private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/premios";

        public async Task<bool> GuardarPremioFirestoreAsync(Premio premio, string id, string idToken)
        {
            var url = $"{FirestoreBaseUrl}/{id}";

            var body = new
            {
                fields = new
                {
                    nombrePremio = new { stringValue = premio.NombrePremio },
                    descripcionPremio = new { stringValue = premio.DescripcionPremio },
                    puntosRequeridos = new { integerValue = premio.PuntosRequeridos },
                    estadoPremio = new { booleanValue = premio.EstadoPremio },
                    fotoPremioUrl = new { stringValue = premio.FotoPremioUrl }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await client.PatchAsync(url, content); // PATCH crea o actualiza

            return response.IsSuccessStatusCode;
        }

        public async Task<List<Premio>> ObtenerPremiosDesdeFirestoreAsync(string idToken)
        {
            var url = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/premios";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<Premio>();

            var json = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(json);

            var lista = new List<Premio>();

            foreach (var doc in document.RootElement.GetProperty("documents").EnumerateArray())
            {
                var fields = doc.GetProperty("fields");

                var premio = new Premio
                {
                    IdPremio = int.Parse(doc.GetProperty("name").ToString().Split('/').Last()),
                    NombrePremio = fields.GetProperty("nombrePremio").GetProperty("stringValue").GetString() ?? "",
                    DescripcionPremio = fields.GetProperty("descripcionPremio").GetProperty("stringValue").GetString() ?? "",
                    PuntosRequeridos = int.Parse(fields.GetProperty("puntosRequeridos").GetProperty("integerValue").GetString() ?? "0"),
                    EstadoPremio = fields.GetProperty("estadoPremio").GetProperty("booleanValue").GetBoolean(),
                    FotoPremioUrl = fields.GetProperty("fotoPremioUrl").GetProperty("stringValue").GetString() ?? "",
                    FotoPremio = "" // lo puedes dejar vacío al inicio, luego se descarga si lo deseas offline
                };

                lista.Add(premio);
            }

            return lista;
        }

    }
}
