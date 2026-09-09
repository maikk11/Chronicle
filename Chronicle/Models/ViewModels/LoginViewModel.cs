using System.ComponentModel.DataAnnotations;

namespace Chronicle.Models.ViewModels;
public class LoginViewModel
{
    [Required]
    public string Username {get;set;} = string.Empty;
    [Required]
    public string Password {get;set;} = string.Empty;
}