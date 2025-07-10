using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace MauiFirebase.Services
{
    public class SupabaseStorageService
    {
        private readonly string _supabaseUrl = "https://vcymbfxfajvetiaxgryo.supabase.co";
        private readonly string _apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZjeW1iZnhmYWp2ZXRpYXhncnlvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTIxMDg0MzEsImV4cCI6MjA2NzY4NDQzMX0.4AByT0iXC1Y1ad83D_BtKJkYgm7APi5CzBtyx4ARgzA";
        private readonly string _bucketName = "imagenesmaui"; // tu bucket

        public async Task<string?> SubirImagenAsync(Stream imagenStream, string rutaCompleta)
        {
            try
            {
                var url = $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{rutaCompleta}";

                var client = new RestClient();
                var request = new RestRequest(url, Method.Post);

                // Headers
                request.AddHeader("apikey", _apiKey);
                request.AddHeader("Authorization", $"Bearer {_apiKey}");
                request.AddHeader("x-upsert", "true"); // Permitir sobrescribir

                // Archivo
                byte[] bytes = imagenStream.ReadAllBytes();
                request.AddBody(bytes, "image/jpeg");

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{rutaCompleta}";
                }
                else
                {
                    Console.WriteLine($"❌ Error al subir: {response.StatusCode} - {response.Content}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al subir imagen: {ex.Message}");
                return null;
            }
        }
    }
    // Extensión para leer bytes del stream
    public static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            using MemoryStream ms = new();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

}
