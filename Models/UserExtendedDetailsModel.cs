using System.ComponentModel.DataAnnotations;
using ASP.NET_Exam.Data.Schema;

namespace ASP.NET_Exam.Models;

public class UserExtendedDetailsModel : UserModel {
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    [Display(Name = "Roles")]
    public List<string>? RoleNames { get; set; } = new();

    [Display(Name = "Specializations")]
    public List<string>? SpecializationIdStrings { get; set; } = new();

    public List<Specialization>? Specializations { get; set; }
}