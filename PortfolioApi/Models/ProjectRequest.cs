using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Models;

public class ProjectRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Titel { get; set; } = string.Empty;

    [Required]
    [StringLength(10000, MinimumLength = 25)]
    public string Beschrijving { get; set; } = string.Empty;
    [Required]
    [StringLength(25)]
    public string Categorie { get; set; } = string.Empty;

    [Required]
    public string GitHubUrl { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
}