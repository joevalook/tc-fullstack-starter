using System.Net;
using System.Net.Http.Json;
using WorkOrders.Api.Dtos;

namespace WorkOrders.Api.Tests.Integration;

public class AuthControllerIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ReturnsOk_AndToken_WhenRequestIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new RegisterRequest
        {
            Email = "integration@example.com",
            Password = "Password123!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.Token));
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new RegisterRequest
        {
            Email = "duplicate@example.com",
            Password = "Password123!"
        };

        await client.PostAsJsonAsync("/api/auth/register", request);

        // Act
        var secondResponse = await client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }
}