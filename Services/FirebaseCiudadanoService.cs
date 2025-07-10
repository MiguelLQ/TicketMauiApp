using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MauiFirebase.Models;

namespace MauiFirebase.Services
{
    public class FirebaseCiudadanoService
    {
        private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/ticketapp-c31cf/databases/(default)/documents/residentes";

        public async Task<bool> GuardarEnFirestoreAsync(Residente residente, string uid, string idToken)
        {
            var url = $"{FirestoreBaseUrl}/{uid}";

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

            var response = await client.PatchAsync(url, content); // PATCH crea o actualiza

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> ResidenteExisteEnFirestoreAsync(string uid, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/ticketapp-c31cf/databases/(default)/documents/residentes/{uid}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await client.GetAsync(url);
            return response.IsSuccessStatusCode;
        }

    }
}
