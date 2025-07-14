using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories;
public class VehiculoRepository : IVehiculoRepository
{
    private readonly AppDatabase _database;
    private readonly UsuarioRepository  _usuariorepository;
    public VehiculoRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<bool> ChangeEstadoVehiculoAsync(string id)
    {
        Vehiculo vehiculo = await _database.Database!.Table<Vehiculo>()
            .Where(v => v.IdVehiculo == id)
            .FirstOrDefaultAsync();
        if (vehiculo == null)
            return false;
        vehiculo.EstadoVehiculo = !vehiculo.EstadoVehiculo;
        int result = await _database.Database.UpdateAsync(vehiculo);
        return result > 0;
    }

    public async Task<Vehiculo> CreateVehiculoAsync(Vehiculo vehiculo)
    {
        int resultado = await _database.Database!.InsertAsync(vehiculo);
        return vehiculo;
    }

    public async Task<List<Vehiculo>> GetAllVehiculoAsync()
    {
        var resultado = await _database.Database!.Table<Vehiculo>().ToListAsync();
        return resultado;
    }

    public Task<Vehiculo?> GetVehiculoByIdAsync(string id)
    {
        var resultado = _database.Database!.Table<Vehiculo>()
            .Where(v => v.IdVehiculo == id)
            .FirstOrDefaultAsync();
        return resultado!;
    }

    public async Task<int> UpdateVehiculoAsync(Vehiculo vehiculo)
    {
        var resultado = await _database.Database!.UpdateAsync(vehiculo);
        return resultado;
    }
    public async Task<Vehiculo?> GetVehiculoPorPlacaAsync(string placa)
    {
        var resultado = await _database.Database!.Table<Vehiculo>()
            .Where(v => v.PlacaVehiculo == placa)
            .FirstOrDefaultAsync();
        return resultado;
    }

    public async Task<List<Vehiculo>> ObtenerVehiculosPorDiaAsync(DayOfWeek dia)
    {
        string diaTexto = TraducirDia(dia); // «Lunes», «Martes», …

        var rutas = await _database.Database!.Table<Ruta>()
                       .Where(r => r.DiasDeRecoleccion != null &&
                                   r.DiasDeRecoleccion.Contains(diaTexto))
                       .ToListAsync();

        if (!rutas.Any())
            return new List<Vehiculo>();

        var vehiculoIds = rutas.Select(r => r.IdVehiculo).Distinct().ToList();

        // 3. Vehículos correspondientes
        var vehiculos = await _database.Database!.Table<Vehiculo>()
                             .Where(v => vehiculoIds.Contains(v.IdVehiculo))
                             .ToListAsync();

        // 4. Rellenar nombre del conductor consultando usuario por UID
        foreach (var v in vehiculos)
        {
            if (v.IdUsuario != null)
            {
                var usuario = await _usuariorepository.ObtenerUsuarioPorUidAsync(v.IdUsuario);
                v.Nombre = usuario?.Nombre ?? "";   // usa la propiedad real de tu modelo Usuario
            }
        }

        return vehiculos;

    }

    private string TraducirDia(DayOfWeek dia) => dia switch
    {
        DayOfWeek.Monday => "Lunes",
        DayOfWeek.Tuesday => "Martes",
        DayOfWeek.Wednesday => "Miércoles",
        DayOfWeek.Thursday => "Jueves",
        DayOfWeek.Friday => "Viernes",
        DayOfWeek.Saturday => "Sábado",
        DayOfWeek.Sunday => "Domingo",
        _ => ""
    };

}
