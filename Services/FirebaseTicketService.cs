using System.Text;
using System.Text.Json;
using MauiFirebase.Models;
namespace MauiFirebase.Services;

public class FirebaseTicketService
{
    private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/sangeronimomuniapp/databases/(default)/documents/tickets";
    public async Task<bool> GuardarTicketFirestoreAsync(Ticket ticket, string id, string idToken)
    {
        var url = $"{FirestoreBaseUrl}/{id}";
        var body = new
        {
            fields = new
            {
                ColorTicket = new { stringValue = ticket.ColorTicket },
                EstadoTicket = new { booleanValue = ticket.EstadoTicket },
                FechaRegistro = new { timestampValue = ticket.FechaRegistro.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }
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

    public async Task<List<Ticket>> ObtenerTicketsDesdeFirestoreAsync(string idToken)
    {
        var url = FirestoreBaseUrl;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Ticket>();
        }
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var lista = new List<Ticket>();
        if (!document.RootElement.TryGetProperty("documents", out var docs))
        {
            return lista;
        }
        foreach (var doc in docs.EnumerateArray())
        {
            var fields = doc.GetProperty("fields");
            var ticket = new Ticket
            {
                IdTicket = doc.GetProperty("name").ToString().Split('/').Last(),
                ColorTicket = fields.GetProperty("ColorTicket").GetProperty("stringValue").GetString(),
                EstadoTicket = fields.GetProperty("EstadoTicket").GetProperty("booleanValue").GetBoolean(),
                FechaRegistro = DateTime.Parse(fields.GetProperty("FechaRegistro").GetProperty("timestampValue").GetString() ?? DateTime.UtcNow.ToString()),
                Sincronizado = true
            };
            lista.Add(ticket);
        }
        return lista;
    }
}
