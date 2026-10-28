using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Models;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/blogposts")]
public class BlogpostsController(IBlogpostService blogposts) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BlogpostResponse>>> GetBlogposts()
    {
        return Ok(await blogposts.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BlogpostResponse>> GetBlogpost(int id)
    {
        var blogpost = await blogposts.GetByIdAsync(id);
        if (blogpost is null)
            return NotFound();

        return Ok(blogpost);
    }

    [HttpPost]
    public async Task<ActionResult<BlogpostResponse>> CreateBlogpost(BlogpostRequest request)
    {
        var blogpost = await blogposts.CreateAsync(request);
        return CreatedAtAction(nameof(GetBlogpost), new { id = blogpost.Id }, blogpost);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBlogpost(int id, BlogpostRequest request)
    {
        if (!await blogposts.UpdateAsync(id, request))
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBlogpost(int id)
    {
        if (!await blogposts.DeleteAsync(id))
            return NotFound();

        return NoContent();
    }
}
