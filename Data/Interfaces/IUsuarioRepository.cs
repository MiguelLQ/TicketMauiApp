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
        Task<string?> AgregarUsuarioAsync(Usuario usuario);

    }
}
