using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortfolioApi.Data;
using System.Net.Http.Json;

namespace PortfolioApi.Tests;

public class PortfolioApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        db.Database.EnsureDeleted();
        db.Database.Migrate();

        return host;
    }

    public async Task<HttpClient> MaakClientAsync(string? rol = null)
    {
        var client = CreateClient();
        if (rol is null)
            return client;

        var email = $"{Guid.NewGuid()}@test.nl";
        var wachtwoord = "Test123!";
        await client.PostAsJsonAsync("/api/auth/register", new { email, wachtwoord });

        if (rol != "Gast")
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);
            await userManager.AddToRoleAsync(user!, rol);
        }

        await client.PostAsJsonAsync("/api/auth/login", new { email, wachtwoord });
        return client;
    }
}