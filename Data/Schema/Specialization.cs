using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Exam.Data.Schema;

public class Specialization {
    [Key]
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }

    public ICollection<ApplicationUserSpecialization>? ApplicationUserSpecialization { get; set; } =
        new List<ApplicationUserSpecialization>();
}