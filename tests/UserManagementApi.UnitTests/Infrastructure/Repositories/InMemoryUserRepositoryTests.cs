using FluentAssertions;
using UserManagementApi.Core.Entities;
using UserManagementApi.Infrastructure.Repositories;
using Xunit;

namespace UserManagementApi.UnitTests.Infrastructure.Repositories;

public class InMemoryUserRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsSeededUsers()
    {
        // Arrange
        var repository = new InMemoryUserRepository();

        // Act
        var users = await repository.GetAllAsync();

        // Assert
        users.Should().NotBeEmpty();
        users.Should().HaveCount(3);
        users.Should().Contain(u => u.Name == "John Doe");
        users.Should().Contain(u => u.Name == "Jane Smith");
        users.Should().Contain(u => u.Name == "Bob Johnson");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var repository = new InMemoryUserRepository();

        // Act
        var user = await repository.GetByIdAsync(1);

        // Assert
        user.Should().NotBeNull();
        user!.Id.Should().Be(1);
        user.Name.Should().Be("John Doe");
        user.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var repository = new InMemoryUserRepository();

        // Act
        var user = await repository.GetByIdAsync(999);

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_AddsUserAndAssignsId()
    {
        // Arrange
        var repository = new InMemoryUserRepository();
        var newUser = new User { Name = "Test User", Email = "test@example.com" };

        // Act
        var addedUser = await repository.AddAsync(newUser);

        // Assert
        addedUser.Should().NotBeNull();
        addedUser.Id.Should().BeGreaterThan(0);
        addedUser.Name.Should().Be("Test User");
        addedUser.Email.Should().Be("test@example.com");
        addedUser.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // Verify user is in the repository
        var retrievedUser = await repository.GetByIdAsync(addedUser.Id);
        retrievedUser.Should().BeEquivalentTo(addedUser);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingUser_UpdatesUser()
    {
        // Arrange
        var repository = new InMemoryUserRepository();
        var existingUser = await repository.GetByIdAsync(1);
        existingUser!.Name = "Updated Name";
        existingUser.Email = "updated@example.com";

        // Act
        var updatedUser = await repository.UpdateAsync(existingUser);

        // Assert
        updatedUser.Should().NotBeNull();
        updatedUser.Id.Should().Be(1);
        updatedUser.Name.Should().Be("Updated Name");
        updatedUser.Email.Should().Be("updated@example.com");
        updatedUser.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // Verify user is updated in the repository
        var retrievedUser = await repository.GetByIdAsync(1);
        retrievedUser.Should().BeEquivalentTo(updatedUser);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new InMemoryUserRepository();
        var nonExistentUser = new User { Id = 999, Name = "Non Existent", Email = "nonexistent@example.com" };

        // Act & Assert
        var act = async () => await repository.UpdateAsync(nonExistentUser);
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("User with ID 999 not found");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingUser_DeletesAndReturnsTrue()
    {
        // Arrange
        var repository = new InMemoryUserRepository();
        var initialCount = (await repository.GetAllAsync()).Count();

        // Act
        var result = await repository.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        
        // Verify user is deleted
        var deletedUser = await repository.GetByIdAsync(1);
        deletedUser.Should().BeNull();
        
        // Verify count decreased
        var remainingUsers = await repository.GetAllAsync();
        remainingUsers.Should().HaveCount(initialCount - 1);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        var repository = new InMemoryUserRepository();
        var initialCount = (await repository.GetAllAsync()).Count();

        // Act
        var result = await repository.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
        
        // Verify count unchanged
        var remainingUsers = await repository.GetAllAsync();
        remainingUsers.Should().HaveCount(initialCount);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingUser_ReturnsTrue()
    {
        // Arrange
        var repository = new InMemoryUserRepository();

        // Act
        var exists = await repository.ExistsAsync(1);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        var repository = new InMemoryUserRepository();

        // Act
        var exists = await repository.ExistsAsync(999);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_MultipleUsers_AssignsIncrementalIds()
    {
        // Arrange
        var repository = new InMemoryUserRepository();
        var user1 = new User { Name = "User 1", Email = "user1@example.com" };
        var user2 = new User { Name = "User 2", Email = "user2@example.com" };

        // Act
        var addedUser1 = await repository.AddAsync(user1);
        var addedUser2 = await repository.AddAsync(user2);

        // Assert
        addedUser1.Id.Should().BeGreaterThan(3); // After seeded users
        addedUser2.Id.Should().Be(addedUser1.Id + 1);
    }
}
