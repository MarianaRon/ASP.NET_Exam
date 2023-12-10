using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_Exam.Data.Schema;

public class ApplicationUserSpecialization {
    [Key]
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }

    [Required]
    public required string SpecializationId { get; set; }

    [ForeignKey("SpecializationId")]
    public Specialization? Specialization { get; set; }
}