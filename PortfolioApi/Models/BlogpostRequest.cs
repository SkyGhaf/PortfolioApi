using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Models;

public class BlogpostRequest
{


    [Required]
    [StringLength(50,MinimumLength = 1)]
    public string Titel { get; set; } = string.Empty;
    [Required]
    [MinLength(10)]
    public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}
