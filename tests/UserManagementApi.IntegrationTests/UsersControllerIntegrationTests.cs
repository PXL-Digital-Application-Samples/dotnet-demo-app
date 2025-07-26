using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using UserManagementApi.Core.Entities;
using Xunit;

namespace UserManagementApi.IntegrationTests;

public class UsersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UsersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnSeededUsers()
    {
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var usersJson = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<User[]>(usersJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        users.Should().NotBeNull();
        users.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetUserById_WithValidId_ShouldReturnUser()
    {
        // Act
        var response = await _client.GetAsync("/api/users/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var userJson = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        user.Should().NotBeNull();
        user!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetUserById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var newUser = new { Name = "Test User", Email = "test@example.com" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", newUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var userJson = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        user.Should().NotBeNull();
        user!.Name.Should().Be("Test User");
        user.Email.Should().Be("test@example.com");
        user.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateUser_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidUser = new { Name = "", Email = "invalid-email" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", invalidUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var updateUser = new { Name = "Updated User", Email = "updated@example.com" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/1", updateUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var userJson = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        user.Should().NotBeNull();
        user!.Name.Should().Be("Updated User");
        user.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task UpdateUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var updateUser = new { Name = "Updated User", Email = "updated@example.com" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/999", updateUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_WithValidId_ShouldDeleteUser()
    {
        // First create a user to delete
        var newUser = new { Name = "To Delete", Email = "delete@example.com" };
        var createResponse = await _client.PostAsJsonAsync("/api/users", newUser);
        var userJson = await createResponse.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await _client.DeleteAsync($"/api/users/{user!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/users/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
