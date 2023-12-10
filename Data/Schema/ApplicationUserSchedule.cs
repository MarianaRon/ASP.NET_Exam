using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_Exam.Data.Schema;

[Table("ApplicationUserSchedules")]
public class ApplicationUserSchedule {
    [Key]
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }

    public required string ScheduleId { get; set; }

    [ForeignKey("ScheduleId")]
    public Schedule? Schedule { get; set; }
}