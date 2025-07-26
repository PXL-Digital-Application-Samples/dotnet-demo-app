using FluentAssertions;
using Moq;
using UserManagementApi.Core.Entities;
using UserManagementApi.Core.Interfaces;
using UserManagementApi.Core.Services;
using Xunit;

namespace UserManagementApi.UnitTests.Core.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new UserService(null!);
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("userRepository");
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new() { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new() { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        };
        _mockUserRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedUsers);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedUsers);
        _mockUserRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var expectedUser = new User { Id = 1, Name = "John Doe", Email = "john@example.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
        _mockUserRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull(int id)
    {
        // Act
        var result = await _userService.GetUserByIdAsync(id);

        // Assert
        result.Should().BeNull();
        _mockUserRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(999);

        // Assert
        result.Should().BeNull();
        _mockUserRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_CreatesAndReturnsUser()
    {
        // Arrange
        var name = "John Doe";
        var email = "JOHN@EXAMPLE.COM";
        var expectedUser = new User { Id = 1, Name = "John Doe", Email = "john@example.com" };
        
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
                          .ReturnsAsync((User user) => { user.Id = 1; return user; });

        // Act
        var result = await _userService.CreateUserAsync(name, email);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com"); // Should be normalized to lowercase
        result.Id.Should().Be(1);
        
        _mockUserRepository.Verify(r => r.AddAsync(It.Is<User>(u => 
            u.Name == "John Doe" && 
            u.Email == "john@example.com" &&
            u.CreatedAt <= DateTime.UtcNow)), Times.Once);
    }

    [Theory]
    [InlineData("", "test@example.com")]
    [InlineData("   ", "test@example.com")]
    [InlineData(null, "test@example.com")]
    public async Task CreateUserAsync_WithInvalidName_ThrowsArgumentException(string name, string email)
    {
        // Act & Assert
        var act = async () => await _userService.CreateUserAsync(name, email);
        await act.Should().ThrowAsync<ArgumentException>()
                 .WithParameterName("name")
                 .WithMessage("Name cannot be empty*");
    }

    [Theory]
    [InlineData("John Doe", "")]
    [InlineData("John Doe", "   ")]
    [InlineData("John Doe", null)]
    public async Task CreateUserAsync_WithInvalidEmail_ThrowsArgumentException(string name, string email)
    {
        // Act & Assert
        var act = async () => await _userService.CreateUserAsync(name, email);
        await act.Should().ThrowAsync<ArgumentException>()
                 .WithParameterName("email")
                 .WithMessage("Email cannot be empty*");
    }

    [Theory]
    [InlineData("John Doe", "invalid-email")]
    [InlineData("John Doe", "invalid@")]
    [InlineData("John Doe", "@invalid.com")]
    public async Task CreateUserAsync_WithInvalidEmailFormat_ThrowsArgumentException(string name, string email)
    {
        // Act & Assert
        var act = async () => await _userService.CreateUserAsync(name, email);
        await act.Should().ThrowAsync<ArgumentException>()
                 .WithParameterName("email")
                 .WithMessage("Invalid email format*");
    }

    [Fact]
    public async Task UpdateUserAsync_WithValidData_UpdatesAndReturnsUser()
    {
        // Arrange
        var existingUser = new User { Id = 1, Name = "Old Name", Email = "old@example.com" };
        var updatedUser = new User { Id = 1, Name = "New Name", Email = "new@example.com", UpdatedAt = DateTime.UtcNow };
        
        _mockUserRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(updatedUser);

        // Act
        var result = await _userService.UpdateUserAsync(1, "New Name", "NEW@EXAMPLE.COM");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("New Name");
        result.Email.Should().Be("new@example.com");
        result.UpdatedAt.Should().NotBeNull();
        
        _mockUserRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => 
            u.Id == 1 && 
            u.Name == "New Name" && 
            u.Email == "new@example.com" &&
            u.UpdatedAt.HasValue)), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UpdateUserAsync_WithInvalidId_ReturnsNull(int id)
    {
        // Act
        var result = await _userService.UpdateUserAsync(id, "Name", "email@example.com");

        // Assert
        result.Should().BeNull();
        _mockUserRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateUserAsync(999, "Name", "email@example.com");

        // Assert
        result.Should().BeNull();
        _mockUserRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData("", "test@example.com")]
    [InlineData("   ", "test@example.com")]
    [InlineData(null, "test@example.com")]
    public async Task UpdateUserAsync_WithInvalidName_ThrowsArgumentException(string name, string email)
    {
        // Act & Assert
        var act = async () => await _userService.UpdateUserAsync(1, name, email);
        await act.Should().ThrowAsync<ArgumentException>()
                 .WithParameterName("name")
                 .WithMessage("Name cannot be empty*");
    }

    [Theory]
    [InlineData("John Doe", "")]
    [InlineData("John Doe", "   ")]
    [InlineData("John Doe", null)]
    public async Task UpdateUserAsync_WithInvalidEmail_ThrowsArgumentException(string name, string email)
    {
        // Act & Assert
        var act = async () => await _userService.UpdateUserAsync(1, name, email);
        await act.Should().ThrowAsync<ArgumentException>()
                 .WithParameterName("email")
                 .WithMessage("Email cannot be empty*");
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidId_DeletesAndReturnsTrue()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockUserRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteUserAsync_WithInvalidId_ReturnsFalse(int id)
    {
        // Act
        var result = await _userService.DeleteUserAsync(id);

        // Assert
        result.Should().BeFalse();
        _mockUserRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserNotFound_ReturnsFalse()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUserAsync(999);

        // Assert
        result.Should().BeFalse();
        _mockUserRepository.Verify(r => r.DeleteAsync(999), Times.Once);
    }
}
