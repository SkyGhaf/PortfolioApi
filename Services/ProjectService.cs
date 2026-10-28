using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Models;

namespace PortfolioApi.Services;

public class ProjectService(PortfolioDbContext db) : IProjectService
{
    public async Task<List<ProjectResponse>> GetAllAsync()
    {
        return await db.Projects
            .Select(p => new ProjectResponse(p.Id, p.Titel, p.Beschrijving, p.Categorie, p.GitHubUrl, p.Datum))
            .ToListAsync();
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id)
    {
        return await db.Projects
            .Where(p => p.Id == id)
            .Select(p => new ProjectResponse(p.Id, p.Titel, p.Beschrijving, p.Categorie, p.GitHubUrl, p.Datum))
            .FirstOrDefaultAsync();
    }

    public async Task<ProjectResponse> CreateAsync(ProjectRequest request)
    {
        var project = new Project
        {
            Titel = request.Titel,
            Beschrijving = request.Beschrijving,
            Categorie = request.Categorie,
            GitHubUrl = request.GitHubUrl,
            Datum = request.Datum
        };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        return new ProjectResponse(project.Id, project.Titel, project.Beschrijving, project.Categorie, project.GitHubUrl, project.Datum);
    }

    public async Task<bool> UpdateAsync(int id, ProjectRequest request)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null)
            return false;

        project.Titel = request.Titel;
        project.Beschrijving = request.Beschrijving;
        project.Categorie = request.Categorie;
        project.GitHubUrl = request.GitHubUrl;
        project.Datum = request.Datum;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null)
            return false;

        db.Projects.Remove(project);
        await db.SaveChangesAsync();
        return true;
    }
}