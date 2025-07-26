using UserManagementApi.Core.Entities;
using UserManagementApi.Core.Interfaces;

namespace UserManagementApi.Infrastructure.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users;
    private int _nextId = 1;

    public InMemoryUserRepository()
    {
        _users = new List<User>
        {
            new() { Id = _nextId++, Name = "John Doe", Email = "john.doe@example.com", CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new() { Id = _nextId++, Name = "Jane Smith", Email = "jane.smith@example.com", CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new() { Id = _nextId++, Name = "Bob Johnson", Email = "bob.johnson@example.com", CreatedAt = DateTime.UtcNow.AddDays(-10) }
        };
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        await Task.Delay(1); // Simulate async operation
        return _users.ToList();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        await Task.Delay(1); // Simulate async operation
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<User> AddAsync(User user)
    {
        await Task.Delay(1); // Simulate async operation
        user.Id = _nextId++;
        user.CreatedAt = DateTime.UtcNow;
        _users.Add(user);
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        await Task.Delay(1); // Simulate async operation
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser == null)
            throw new InvalidOperationException($"User with ID {user.Id} not found");

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.UpdatedAt = DateTime.UtcNow;

        return existingUser;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await Task.Delay(1); // Simulate async operation
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return false;

        _users.Remove(user);
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        await Task.Delay(1); // Simulate async operation
        return _users.Any(u => u.Id == id);
    }
}
