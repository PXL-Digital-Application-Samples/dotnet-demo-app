using UserManagementApi.Core.Entities;

namespace UserManagementApi.Core.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(string name, string email);
    Task<User?> UpdateUserAsync(int id, string name, string email);
    Task<bool> DeleteUserAsync(int id);
}
