using Microsoft.EntityFrameworkCore;
using roster_api_app.Entities;

namespace roster_api_app.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Housekeeper> Housekeepers { get; set; }
    public DbSet<Resident> Residents { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<CleaningTask> CleaningTasks { get; set; }
    public DbSet<Roster> Rosters { get; set; }
    public DbSet<RosterTask> RosterTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<RosterTask>()
            .HasOne(rt => rt.Roster)
            .WithMany(r => r.RosterTasks)
            .HasForeignKey(rt => rt.RosterId);

        modelBuilder.Entity<RosterTask>()
            .HasOne(rt => rt.Housekeeper)
            .WithMany()
            .HasForeignKey(rt => rt.HousekeeperId);

        modelBuilder.Entity<RosterTask>()
            .HasOne(rt => rt.Task)
            .WithMany()
            .HasForeignKey(rt => rt.TaskId);

        modelBuilder.Entity<RosterTask>()
            .HasOne(rt => rt.Location)
            .WithMany()
            .HasForeignKey(rt => rt.LocationId);

        modelBuilder.Entity<RosterTask>()
            .HasOne(rt => rt.Resident)
            .WithMany()
            .HasForeignKey(rt => rt.ResidentId);
    }
}