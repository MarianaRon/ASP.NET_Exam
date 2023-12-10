using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Exam.Data.Schema;

public class ApplicationUser : IdentityUser {
    public ApplicationUser() { }

    public ApplicationUser(string userName) : base(userName) { }

    public DateOnly BirthDate { get; set; }

    public string? Gender { get; set; }

    public ICollection<ApplicationUserSchedule>? ApplicationUserSchedules { get; set; } =
        new List<ApplicationUserSchedule>();

    public ICollection<ApplicationUserSpecialization>? ApplicationUserSpecialization { get; set; } =
        new List<ApplicationUserSpecialization>();
}