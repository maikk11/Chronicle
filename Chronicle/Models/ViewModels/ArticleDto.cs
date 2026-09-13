using Chronicle.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace Chronicle.Models.ViewModels;
public class ArticleDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime? PublishDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool? IsAccepted { get; set; }
    public IdentityUser? User { get; set; }
    public Category? Category { get; set; }
    public Image? Image { get; set; }
}