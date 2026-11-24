using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortfolioApi.Data;
using System.Net.Http.Json;
using System.Security.Claims;

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

    public async Task<HttpClient> MaakClientAsync(string? rol = null, bool magVerwijderen = false)
    {
        var client = CreateClient();
        if (rol is null)
            return client;

        var email = $"{Guid.NewGuid()}@test.nl";
        var password = "Test123!";
        await client.PostAsJsonAsync("/api/auth/register", new { email, password });

        if (rol != "Gast" || magVerwijderen)
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);

            if (rol != "Gast")
                await userManager.AddToRoleAsync(user!, rol);

            if (magVerwijderen)
                await userManager.AddClaimAsync(user!, new Claim("MagVerwijderen", "ja"));
        }

        await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        return client;
    }
}