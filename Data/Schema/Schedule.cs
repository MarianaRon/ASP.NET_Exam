using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_Exam.Data.Schema;

public class Schedule {
    [Key]
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public int MaxGroupSize { get; set; }

    [Display(Name = "Group size")]
    public int GroupSize { get; set; }

    [Display(Name = "Start")]
    public DateTime StartDateTime { get; set; }

    public required string TrainerId { get; set; }

    [ForeignKey("TrainerId")]
    public ApplicationUser? Trainer { get; set; }

    public required string HallId { get; set; }

    [ForeignKey("HallId")]
    public Hall? Hall { get; set; }

    public ICollection<ApplicationUserSchedule>? ApplicationUserSchedules { get; set; } =
        new List<ApplicationUserSchedule>();
}