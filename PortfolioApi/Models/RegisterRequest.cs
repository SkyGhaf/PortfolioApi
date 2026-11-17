namespace PortfolioApi.Models;

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Wachtwoord { get; set; }
}