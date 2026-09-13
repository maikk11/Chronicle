using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Identity;

namespace Chronicle.Models.Domain;
public class Article
{
    public long Id {get;set;}
    [Required]
    [MaxLength(100)]
    public string Title {get;set;} = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Subtitle {get;set;} = string.Empty;
    [Required]
    public string Body {get;set;} = string.Empty;
    // null means article not published yet
    public DateTime? PublishDate {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
    [ValidateNever]
    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    [ValidateNever]
    public IdentityUser? User { get; set; }

    [Required(ErrorMessage = "The category is mandatory")]
    [Range(1, long.MaxValue, ErrorMessage = "Select a valid category")]
    public long? CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    [ValidateNever]
    public Category? Category { get; set; }
    [ValidateNever]
    public Image? Image { get;set; }
    public bool? IsAccepted { get; set; }
}