using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Chronicle.Models.Domain;

public class CareerRequest
{
    public long Id {get;set;}
    [Required]
    [MaxLength(1000)]
    public string Body {get;set;} = string.Empty;
    public bool IsChecked {get;set;}
    public bool? IsApproved {get;set;}
    [ForeignKey("User")]
    [ValidateNever]
    public string UserId {get;set;} = string.Empty;
    public IdentityUser? User {get;set;}
    [ForeignKey("Role")]
    [Required]
    public string RoleId {get;set;} = string.Empty;
    public IdentityRole? Role {get;set;}
}