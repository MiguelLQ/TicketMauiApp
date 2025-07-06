using MauiFirebase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiFirebase.Data.Interfaces;
public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user);
    Task<int> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}
