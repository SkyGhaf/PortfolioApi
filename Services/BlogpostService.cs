using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Models;

namespace PortfolioApi.Services;

public class BlogpostService(PortfolioDbContext db) : IBlogpostService
{
    public async Task<List<BlogpostResponse>> GetAllAsync()
    {
        return await db.Blogposts
            .Select(b => new BlogpostResponse(b.Id, b.Titel, b.Inhoud, b.Publicatiedatum))
            .ToListAsync();
    }

    public async Task<BlogpostResponse?> GetByIdAsync(int id)
    {
        return await db.Blogposts
            .Where(b => b.Id == id)
            .Select(b => new BlogpostResponse(b.Id, b.Titel, b.Inhoud, b.Publicatiedatum))
            .FirstOrDefaultAsync();
    }

    public async Task<BlogpostResponse> CreateAsync(BlogpostRequest request)
    {
        var blogpost = new Blogpost
        {
            Titel = request.Titel,
            Inhoud = request.Inhoud,
            Publicatiedatum = request.Publicatiedatum
        };
        db.Blogposts.Add(blogpost);
        await db.SaveChangesAsync();

        return new BlogpostResponse(blogpost.Id, blogpost.Titel, blogpost.Inhoud, blogpost.Publicatiedatum);
    }

    public async Task<bool> UpdateAsync(int id, BlogpostRequest request)
    {
        var blogpost = await db.Blogposts.FindAsync(id);
        if (blogpost is null)
            return false;

        blogpost.Titel = request.Titel;
        blogpost.Inhoud = request.Inhoud;
        blogpost.Publicatiedatum = request.Publicatiedatum;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var blogpost = await db.Blogposts.FindAsync(id);
        if (blogpost is null)
            return false;

        db.Blogposts.Remove(blogpost);
        await db.SaveChangesAsync();
        return true;
    }
}