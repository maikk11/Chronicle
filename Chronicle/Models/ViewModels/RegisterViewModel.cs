using System.ComponentModel.DataAnnotations;

namespace Chronicle.Models.ViewModels;
public class RegisterViewModel
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9\-._@+]+$",
    ErrorMessage = "Username can only contain letters, digits and - . _ @ +")]
    public string Username {get;set;} = string.Empty;
    [Required]
    [EmailAddress]
    public string Email {get;set;} = string.Empty;
    [Required]
    [MinLength(6)]
    public string Password {get;set;} = string.Empty;
}