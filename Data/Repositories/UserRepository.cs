using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
using SQLite;

namespace MauiFirebase.Data.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDatabase _database;

    public UserRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _database.Database!.Table<User>().ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _database.Database!.Table<User>()
            .Where(u => u.IdUser == id)
            .FirstOrDefaultAsync();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await _database.Database!.InsertAsync(user);
        return user;
    }

    public async Task<int> UpdateUserAsync(User user)
    {
        return await _database.Database!.UpdateAsync(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await GetUserByIdAsync(id);
        if (user == null)
            return false;
        await _database.Database!.DeleteAsync(user);
        return true;
    }
}
