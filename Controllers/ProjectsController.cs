using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Models;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(IProjectService projects) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectResponse>>> GetProjects()
    {
        return Ok(await projects.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectResponse>> GetProject(int id)
    {
        var project = await projects.GetByIdAsync(id);
        if (project is null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> CreateProject(ProjectRequest request)
    {
        var project = await projects.CreateAsync(request);
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProject(int id, ProjectRequest request)
    {
        if (!await projects.UpdateAsync(id, request))
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        if (!await projects.DeleteAsync(id))
            return NotFound();

        return NoContent();
    }
}