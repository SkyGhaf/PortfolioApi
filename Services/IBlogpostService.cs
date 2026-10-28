using PortfolioApi.Models;

namespace PortfolioApi.Services;

public interface IBlogpostService
{
    Task<List<BlogpostResponse>> GetAllAsync();
    Task<BlogpostResponse?> GetByIdAsync(int id);
    Task<BlogpostResponse> CreateAsync(BlogpostRequest request);
    Task<bool> UpdateAsync(int id, BlogpostRequest request);
    Task<bool> DeleteAsync(int id);
}
