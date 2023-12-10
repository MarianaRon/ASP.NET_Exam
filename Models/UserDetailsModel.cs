using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Exam.Models;

public class UserDetailsModel {
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    [Display(Name = "UserName")]
    public required string? UserName { get; set; }

    [Display(Name = "Roles")]
    public List<string>? RoleNames { get; set; } = new();

    [Display(Name = "Specializations")]
    public List<string>? SpecializationIdStrings { get; set; } = new();
}