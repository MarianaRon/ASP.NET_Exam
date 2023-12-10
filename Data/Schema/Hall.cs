using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_Exam.Data.Schema;

[Table("Halls")]
public class Hall {
    [Key]
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }

    public required string ManagerId { get; set; }

    [ForeignKey("ManagerId")]
    public ApplicationUser? Manager { get; set; }

    public ICollection<Schedule>? Schedules { get; set; } =
        new List<Schedule>();
}