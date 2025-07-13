using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetUsuariosAsync();
        Task<bool> AgregarUsuarioAsync(Usuario usuario);
        Task<bool> EditarUsuarioLocalAsync(Usuario usuario);
        Task<Usuario?> ObtenerUsuarioPorUidAsync(string uid);

    }
}
