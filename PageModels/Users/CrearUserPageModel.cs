using CommunityToolkit.Mvvm.ComponentModel;
using MauiFirebase.Data.Interfaces;

namespace MauiFirebase.PageModels.Users;
public partial class CrearUserPageModel: ObservableObject
{
    private readonly IUserRepository _userRepository;
    [ObservableProperty]
    private string? _nombreUsuario;
    [ObservableProperty]
    private string? _passwordUsuario;
    [ObservableProperty]
    private bool _estadoUsuario = true;

   


    public CrearUserPageModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
}
