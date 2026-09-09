using System.ComponentModel.DataAnnotations;
namespace Chronicle.Models.Domain;
public class Category
{
    public long Id {get; set;}
    [Required]
    [MaxLength(50)]
    public string Name {get;set;} = string.Empty;
    public ICollection<Article> Articles {get;set;} = new List<Article>();
}