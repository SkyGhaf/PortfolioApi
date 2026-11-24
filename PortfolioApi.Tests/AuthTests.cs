using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApi.Models;

namespace PortfolioApi.Tests;

[Collection("Api")]
public class AuthTests(PortfolioApiFactory factory)
{
    [Fact]
    public async Task Register_NieuweGebruiker_KrijgtRolGast()
    {
        // Arrange
        var client = factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.nl";

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", new { email, password = "Test123!" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);
        Assert.True(await userManager.IsInRoleAsync(user, "Gast"));
    }

    [Fact]
    public async Task Register_ZwakWachtwoord_Geeft400()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", new { email = $"{Guid.NewGuid()}@test.nl", password = "123" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_JuistWachtwoord_GeeftCookie()
    {
        // Arrange
        var client = factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.nl";
        await client.PostAsJsonAsync("/api/auth/register", new { email, password = "Test123!" });

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password = "Test123!" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }

    [Fact]
    public async Task Login_FoutWachtwoord_Geeft401()
    {
        // Arrange
        var client = factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.nl";
        await client.PostAsJsonAsync("/api/auth/register", new { email, password = "Test123!" });

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password = "FoutWachtwoord1!" });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_Ingelogd_GeeftRollen()
    {
        // Arrange
        var client = await factory.MaakClientAsync("Editor");

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sessie = await response.Content.ReadFromJsonAsync<SessionDto>();
        Assert.Contains("Editor", sessie!.Roles);
    }

    [Fact]
    public async Task Me_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}