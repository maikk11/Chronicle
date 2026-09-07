using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
    // null means article not published yet
    public DateTime? PublishDate {get;set;}
    public long CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }
}