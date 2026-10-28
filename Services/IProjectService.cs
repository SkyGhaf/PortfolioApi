using PortfolioApi.Models;

namespace PortfolioApi.Services;

public interface IProjectService
{
    Task<List<ProjectResponse>> GetAllAsync();
    Task<ProjectResponse?> GetByIdAsync(int id);
    Task<ProjectResponse> CreateAsync(ProjectRequest request);
    Task<bool> UpdateAsync(int id, ProjectRequest request);
    Task<bool> DeleteAsync(int id);
}
