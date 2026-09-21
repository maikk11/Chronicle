using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Chronicle.Models.Domain;
public class Image
{
    public long Id {get; set;}
    [Required]
    public string Path {get;set;} = string.Empty;
    public long ArticleId { get; set; }
    [ForeignKey("ArticleId")]
    [ValidateNever]
    public Article? Article { get; set; }
}