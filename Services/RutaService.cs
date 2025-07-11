using System.Net.Http;
using System.Text.Json;
using Microsoft.Maui.Controls.Maps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RutaService
{
    private readonly string _apiKey = "AIzaSyBtD3WDrurc_whiHRAd0odrge26SaUrKfs"; // Pon aquí tu API Key

    public async Task<List<Location>> ObtenerRutaGoogleAsync(List<Location> puntos)
    {
        if (puntos.Count < 2)
            return new List<Location>();

        var origin = $"{puntos[0].Latitude},{puntos[0].Longitude}";
        var destination = $"{puntos[^1].Latitude},{puntos[^1].Longitude}";
        var waypoints = string.Join("|", puntos.Skip(1).Take(puntos.Count - 2)
            .Select(p => $"{p.Latitude},{p.Longitude}"));

        string url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin}&destination={destination}&waypoints={waypoints}&key={_apiKey}";

        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(url);

        using var jsonDoc = JsonDocument.Parse(response);
        var points = new List<Location>();

        var routes = jsonDoc.RootElement.GetProperty("routes");
        if (routes.GetArrayLength() > 0)
        {
            var overviewPolyline = routes[0].GetProperty("overview_polyline").GetProperty("points").GetString();
            points = DecodePolyline(overviewPolyline);
        }

        return points;
    }

    // Método para decodificar polyline codificado por Google
    public List<Location> DecodePolyline(string encodedPoints)
    {
        if (string.IsNullOrEmpty(encodedPoints))
            return new List<Location>();

        var poly = new List<Location>();
        int index = 0, len = encodedPoints.Length;
        int lat = 0, lng = 0;

        while (index < len)
        {
            int b, shift = 0, result = 0;
            do
            {
                b = encodedPoints[index++] - 63;
                result |= (b & 0x1f) << shift;
                shift += 5;
            } while (b >= 0x20);
            int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
            lat += dlat;

            shift = 0;
            result = 0;
            do
            {
                b = encodedPoints[index++] - 63;
                result |= (b & 0x1f) << shift;
                shift += 5;
            } while (b >= 0x20);
            int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
            lng += dlng;

            var p = new Location(lat / 1E5, lng / 1E5);
            poly.Add(p);
        }

        return poly;
    }
}
