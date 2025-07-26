using UserManagementApi.Core.Entities;
using UserManagementApi.Core.Interfaces;

namespace UserManagementApi.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        if (id <= 0)
            return null;

        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User> CreateUserAsync(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        var user = new User
        {
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            CreatedAt = DateTime.UtcNow
        };

        return await _userRepository.AddAsync(user);
    }

    public async Task<User?> UpdateUserAsync(int id, string name, string email)
    {
        if (id <= 0)
            return null;

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
            return null;

        existingUser.UpdateDetails(name.Trim(), email.Trim().ToLowerInvariant());
        
        return await _userRepository.UpdateAsync(existingUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        if (id <= 0)
            return false;

        return await _userRepository.DeleteAsync(id);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
