using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkOrders.Api.Data;
using WorkOrders.Api.Dtos;
using WorkOrders.Api.Models;
using WorkOrders.Api.Services;
using WorkOrders.Api.Services.Interfaces;

namespace WorkOrders.Api.Tests.Services;

public class AuthServiceTests
{
    private AppDbContext BuildDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsAuthResponse_WhenEmailIsNew()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(RegisterAsync_ReturnsAuthResponse_WhenEmailIsNew));

        var passwordHasher = new PasswordHasher<User>();

        var tokenService = new Mock<ITokenService>();
        tokenService
            .Setup(x => x.CreateToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");

        var service = new AuthService(
            db,
            passwordHasher,
            tokenService.Object
        );

        var request = new RegisterRequest
        {
            Email = "joe@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await service.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("fake-jwt-token", result.Token);

        var userInDb = await db.Users.FirstOrDefaultAsync(x => x.Email == "joe@example.com");

        Assert.NotNull(userInDb);
        Assert.Equal("joe@example.com", userInDb.Email);
        Assert.False(string.IsNullOrWhiteSpace(userInDb.PasswordHash));

        tokenService.Verify(
            x => x.CreateToken(It.IsAny<User>()),
            Times.Once
        );
    }

    [Fact]
    public async Task RegisterAsync_ReturnsNull_WhenEmailAlreadyExists()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(RegisterAsync_ReturnsNull_WhenEmailAlreadyExists));

        db.Users.Add(new User
        {
            Email = "joe@example.com",
            PasswordHash = "already-hashed"
        });

        await db.SaveChangesAsync();

        var passwordHasher = new PasswordHasher<User>();

        var tokenService = new Mock<ITokenService>();

        var service = new AuthService(
            db,
            passwordHasher,
            tokenService.Object
        );

        var request = new RegisterRequest
        {
            Email = "joe@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await service.RegisterAsync(request);

        // Assert
        Assert.Null(result);
        Assert.Equal(1, await db.Users.CountAsync());

        tokenService.Verify(
            x => x.CreateToken(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task LoginAsync_ReturnsAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(LoginAsync_ReturnsAuthResponse_WhenCredentialsAreValid));

        var passwordHasher = new PasswordHasher<User>();

        var user = new User
        {
            Email = "joe@example.com"
        };

        user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var tokenService = new Mock<ITokenService>();
        tokenService
            .Setup(x => x.CreateToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");

        var service = new AuthService(
            db,
            passwordHasher,
            tokenService.Object
        );

        var request = new LoginRequest
        {
            Email = "joe@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("fake-jwt-token", result.Token);

        tokenService.Verify(
            x => x.CreateToken(It.IsAny<User>()),
            Times.Once
        );
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenPasswordIsWrong()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(LoginAsync_ReturnsNull_WhenPasswordIsWrong));

        var passwordHasher = new PasswordHasher<User>();

        var user = new User
        {
            Email = "joe@example.com"
        };

        user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var tokenService = new Mock<ITokenService>();

        var service = new AuthService(
            db,
            passwordHasher,
            tokenService.Object
        );

        var request = new LoginRequest
        {
            Email = "joe@example.com",
            Password = "WrongPassword!"
        };

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.Null(result);

        tokenService.Verify(
            x => x.CreateToken(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        await using var db = BuildDbContext(nameof(LoginAsync_ReturnsNull_WhenUserDoesNotExist));

        var passwordHasher = new PasswordHasher<User>();
        var tokenService = new Mock<ITokenService>();

        var service = new AuthService(
            db,
            passwordHasher,
            tokenService.Object
        );

        var request = new LoginRequest
        {
            Email = "missing@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.Null(result);

        tokenService.Verify(
            x => x.CreateToken(It.IsAny<User>()),
            Times.Never
        );
    }
}