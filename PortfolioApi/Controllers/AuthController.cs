using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Models;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(CredentialsDto request)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await userManager.AddToRoleAsync(user, "Gast");
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(CredentialsDto request)
    {
        var result = await signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);
        if (!result.Succeeded)
            return Unauthorized();

        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new SessionDto(User.Identity!.Name!,
        User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()));
}