using System.Net;
using System.Net.Http.Json;
using PortfolioApi.Models;

namespace PortfolioApi.Tests;

[Collection("Api")]
public class ProjectsTests(PortfolioApiFactory factory)
{
    private static ProjectRequest NieuwProject(string titel)
    {
        return new ProjectRequest
        {
            Titel = titel,
            Beschrijving = "Een beschrijving van minimaal vijfentwintig tekens.",
            Categorie = "Web",
            GitHubUrl = "https://github.com/SkyGhaf/test",
            Datum = new DateTime(2026, 1, 1)
        };
    }

    private async Task<ProjectResponse> MaakProjectAsync(string titel)
    {
        var editor = await factory.MaakClientAsync("Editor");
        var response = await editor.PostAsJsonAsync("/api/projects", NieuwProject(titel));
        return (await response.Content.ReadFromJsonAsync<ProjectResponse>())!;
    }

    [Fact]
    public async Task GetAll_ZonderLogin_GeeftLijstMetProjecten()
    {
        // Arrange
        var project = await MaakProjectAsync("Project voor lijst");
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var projecten = await response.Content.ReadFromJsonAsync<List<ProjectResponse>>();
        Assert.Contains(projecten!, p => p.Id == project.Id);
    }

    [Fact]
    public async Task GetById_BestaandId_GeeftProject()
    {
        // Arrange
        var project = await MaakProjectAsync("Project op id");
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.GetAsync($"/api/projects/{project.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var opgehaald = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        Assert.Equal("Project op id", opgehaald!.Titel);
    }

    [Fact]
    public async Task GetById_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.GetAsync("/api/projects/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", NieuwProject("Mag niet"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_AlsGast_Geeft403()
    {
        // Arrange
        var client = await factory.MaakClientAsync("Gast");

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", NieuwProject("Mag niet"));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Post_MetSchrijfrol_MaaktProjectAan(string rol)
    {
        // Arrange
        var client = await factory.MaakClientAsync(rol);

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", NieuwProject("Nieuw project"));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        var opgehaald = await client.GetFromJsonAsync<ProjectResponse>($"/api/projects/{project!.Id}");
        Assert.Equal("Nieuw project", opgehaald!.Titel);
    }

    [Fact]
    public async Task Post_OngeldigeInvoer_Geeft400()
    {
        // Arrange
        var client = await factory.MaakClientAsync("Editor");

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", NieuwProject(""));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ZonderLogin_Geeft401()
    {
        // Arrange
        var project = await MaakProjectAsync("Project");
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{project.Id}", NieuwProject("Mag niet"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Put_AlsGast_Geeft403()
    {
        // Arrange
        var project = await MaakProjectAsync("Project");
        var client = await factory.MaakClientAsync("Gast");

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{project.Id}", NieuwProject("Mag niet"));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Put_MetSchrijfrol_WijzigtProject(string rol)
    {
        // Arrange
        var project = await MaakProjectAsync("Oude titel");
        var client = await factory.MaakClientAsync(rol);

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{project.Id}", NieuwProject("Nieuwe titel"));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var opgehaald = await client.GetFromJsonAsync<ProjectResponse>($"/api/projects/{project.Id}");
        Assert.Equal("Nieuwe titel", opgehaald!.Titel);
    }

    [Fact]
    public async Task Put_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await factory.MaakClientAsync("Editor");

        // Act
        var response = await client.PutAsJsonAsync("/api/projects/999999", NieuwProject("Bestaat niet"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ZonderLogin_Geeft401()
    {
        // Arrange
        var project = await MaakProjectAsync("Project");
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.DeleteAsync($"/api/projects/{project.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AlsGast_Geeft403()
    {
        // Arrange
        var project = await MaakProjectAsync("Project");
        var client = await factory.MaakClientAsync("Gast");

        // Act
        var response = await client.DeleteAsync($"/api/projects/{project.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_EditorZonderClaim_Geeft403()
    {
        // Arrange
        var project = await MaakProjectAsync("Project");
        var client = await factory.MaakClientAsync("Editor");

        // Act
        var response = await client.DeleteAsync($"/api/projects/{project.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Delete_MetSchrijfrolEnClaim_VerwijdertProject(string rol)
    {
        // Arrange
        var project = await MaakProjectAsync("Wordt verwijderd");
        var client = await factory.MaakClientAsync(rol, magVerwijderen: true);

        // Act
        var response = await client.DeleteAsync($"/api/projects/{project.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var opgehaald = await client.GetAsync($"/api/projects/{project.Id}");
        Assert.Equal(HttpStatusCode.NotFound, opgehaald.StatusCode);
    }

    [Fact]
    public async Task Delete_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await factory.MaakClientAsync("Editor", magVerwijderen: true);

        // Act
        var response = await client.DeleteAsync("/api/projects/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}