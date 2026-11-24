using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Models;

public sealed class CredentialsDto
{
    [Required, EmailAddress, StringLength(254)]
    public string Email { get; init; } = "";

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; init; } = "";
}

public sealed record SessionDto(string Email, string[] Roles);