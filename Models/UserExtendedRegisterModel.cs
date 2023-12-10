using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Exam.Models;

public class UserExtendedRegisterModel : UserRegisterModel {
    [Display(Name = "Roles")]
    public List<string>? RoleNames { get; set; } = new();

    [Display(Name = "Specializations")]
    public List<string>? SpecializationIdStrings { get; set; } = new();
}