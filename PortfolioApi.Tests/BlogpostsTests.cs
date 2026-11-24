using System.Net;
using System.Net.Http.Json;
using PortfolioApi.Models;

namespace PortfolioApi.Tests;

[Collection("Api")]
public class BlogpostsTests(PortfolioApiFactory factory)
{
    private static BlogpostRequest NieuweBlogpost(string titel)
    {
        return new BlogpostRequest
        {
            Titel = titel,
            Inhoud = "Dit is de inhoud van een testblogpost.",
            Publicatiedatum = new DateTime(2026, 1, 1)
        };
    }

    private async Task<BlogpostResponse> MaakBlogpostAsync(string titel)
    {
        var editor = await factory.MaakClientAsync("Editor");
        var response = await editor.PostAsJsonAsync("/api/blogposts", NieuweBlogpost(titel));
        return (await response.Content.ReadFromJsonAsync<BlogpostResponse>())!;
    }

    [Fact]
    public async Task GetAll_ZonderLogin_GeeftLijstMetBlogposts()
    {
        // Arrange
        var blogpost = await MaakBlogpostAsync("Blogpost voor lijst");
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var blogposts = await response.Content.ReadFromJsonAsync<List<BlogpostResponse>>();
        Assert.Contains(blogposts!, b => b.Id == blogpost.Id);
    }

    [Fact]
    public async Task GetById_BestaandId_GeeftBlogpost()
    {
        // Arrange
        var blogpost = await MaakBlogpostAsync("Blogpost op id");
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.GetAsync($"/api/blogposts/{blogpost.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var opgehaald = await response.Content.ReadFromJsonAsync<BlogpostResponse>();
        Assert.Equal("Blogpost op id", opgehaald!.Titel);
    }

    [Fact]
    public async Task GetById_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.GetAsync("/api/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", NieuweBlogpost("Mag niet"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_AlsGast_Geeft403()
    {
        // Arrange
        var client = await factory.MaakClientAsync("Gast");

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", NieuweBlogpost("Mag niet"));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Post_MetSchrijfrol_MaaktBlogpostAan(string rol)
    {
        // Arrange
        var client = await factory.MaakClientAsync(rol);

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", NieuweBlogpost("Nieuwe blogpost"));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var blogpost = await response.Content.ReadFromJsonAsync<BlogpostResponse>();
        var opgehaald = await client.GetFromJsonAsync<BlogpostResponse>($"/api/blogposts/{blogpost!.Id}");
        Assert.Equal("Nieuwe blogpost", opgehaald!.Titel);
    }

    [Fact]
    public async Task Post_OngeldigeInvoer_Geeft400()
    {
        // Arrange
        var client = await factory.MaakClientAsync("Editor");

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", NieuweBlogpost(""));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ZonderLogin_Geeft401()
    {
        // Arrange
        var blogpost = await MaakBlogpostAsync("Blogpost");
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{blogpost.Id}", NieuweBlogpost("Mag niet"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Put_AlsGast_Geeft403()
    {
        // Arrange
        var blogpost = await MaakBlogpostAsync("Blogpost");
        var client = await factory.MaakClientAsync("Gast");

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{blogpost.Id}", NieuweBlogpost("Mag niet"));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Put_MetSchrijfrol_WijzigtBlogpost(string rol)
    {
        // Arrange
        var blogpost = await MaakBlogpostAsync("Oude titel");
        var client = await factory.MaakClientAsync(rol);

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{blogpost.Id}", NieuweBlogpost("Nieuwe titel"));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var opgehaald = await client.GetFromJsonAsync<BlogpostResponse>($"/api/blogposts/{blogpost.Id}");
        Assert.Equal("Nieuwe titel", opgehaald!.Titel);
    }

    [Fact]
    public async Task Put_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await factory.MaakClientAsync("Editor");

        // Act
        var response = await client.PutAsJsonAsync("/api/blogposts/999999", NieuweBlogpost("Bestaat niet"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ZonderLogin_Geeft401()
    {
        // Arrange
        var blogpost = await MaakBlogpostAsync("Blogpost");
        var client = await factory.MaakClientAsync();

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{blogpost.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AlsGast_Geeft403()
    {
        // Arrange
        var blogpost = await MaakBlogpostAsync("Blogpost");
        var client = await factory.MaakClientAsync("Gast");

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{blogpost.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_EditorZonderClaim_Geeft403()
    {
        // Arrange
        var blogpost = await MaakBlogpostAsync("Blogpost");
        var client = await factory.MaakClientAsync("Editor");

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{blogpost.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Delete_MetSchrijfrolEnClaim_VerwijdertBlogpost(string rol)
    {
        // Arrange
        var blogpost = await MaakBlogpostAsync("Wordt verwijderd");
        var client = await factory.MaakClientAsync(rol, magVerwijderen: true);

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{blogpost.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var opgehaald = await client.GetAsync($"/api/blogposts/{blogpost.Id}");
        Assert.Equal(HttpStatusCode.NotFound, opgehaald.StatusCode);
    }

    [Fact]
    public async Task Delete_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await factory.MaakClientAsync("Editor", magVerwijderen: true);

        // Act
        var response = await client.DeleteAsync("/api/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}