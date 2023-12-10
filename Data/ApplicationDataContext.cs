using ASP.NET_Exam.Data.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Exam.Data;

public class ApplicationDataContext : IdentityDbContext<ApplicationUser> 
{
    //делегуємо конструктор,  який приймає options, базовому класу
    public ApplicationDataContext(DbContextOptions options) : base(options) { }

    
    public ApplicationDataContext() { }

    public DbSet<ApplicationUserSpecialization>? ApplicationUserSpecifications { get; set; }
    public DbSet<Specialization>? Specializations { get; set; }
    public DbSet<ApplicationUserSchedule>? ApplicationUserSchedules { get; set; }
    public DbSet<Schedule>? Schedules { get; set; }
    public DbSet<Hall>? Halls { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.Entity<ApplicationUserSpecialization>()
            .HasOne(aus => aus.User)
            .WithMany(u => u.ApplicationUserSpecialization)
            .HasForeignKey(asp => asp.UserId)
            .HasPrincipalKey(u => u.Id);

        builder.Entity<ApplicationUserSpecialization>()
            .HasOne(aus => aus.Specialization)
            .WithMany(s => s.ApplicationUserSpecialization)
            .HasForeignKey(aus => aus.SpecializationId)
            .HasPrincipalKey(s => s.Id);

        builder.Entity<ApplicationUserSchedule>()
            .HasOne(aus => aus.User)
            .WithMany(u => u.ApplicationUserSchedules)
            .HasForeignKey(aus => aus.UserId)
            .HasPrincipalKey(u => u.Id);

        builder.Entity<ApplicationUserSchedule>()
            .HasOne(aus => aus.Schedule)
            .WithMany(s => s.ApplicationUserSchedules)
            .HasForeignKey(aus => aus.ScheduleId)
            .HasPrincipalKey(s => s.Id);

        // builder.Entity<Schedule>()
        //     .HasOne(s => s.Hall)
        //     .WithMany()
        //     .HasForeignKey(s => s.HallId)
        //     .HasPrincipalKey(h => h.Id);

        builder.Entity<Hall>()
            .HasOne(s => s.Manager)
            .WithMany()
            .HasForeignKey(s => s.ManagerId)
            .HasPrincipalKey(u => u.Id);

        base.OnModelCreating(builder);
    }
}