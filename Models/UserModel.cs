using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Exam.Models;

public class UserModel {
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Phone number")]
    public required string PhoneNumber { get; set; }

    [Required]
    [Display(Name = "UserName")]
    public required string UserName { get; set; }

    [Required]
    [Display(Name = "Birth date")]
    public DateOnly BirthDate { get; set; }

    [Display(Name = "Gender")]
    public string? Gender { get; set; }
}