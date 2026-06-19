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
    public DbSet<LocationType> LocationTypes { get; set; }
    public DbSet<BuildingBlock> BuildingBlocks { get; set; }
    public DbSet<Floor> Floors { get; set; }
    public DbSet<CommonArea> CommonAreas { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<CleaningTask> CleaningTasks { get; set; }
    public DbSet<Roster> Rosters { get; set; }
    public DbSet<RosterTask> RosterTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CleaningTask>()
            .Property(ct => ct.TaskCategory)
            .HasDefaultValue(Entities.Enums.CleaningTaskCategory.CommunityArea);

        modelBuilder.Entity<LocationType>()
            .Property(lt => lt.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<LocationType>()
            .HasIndex(lt => lt.Name)
            .IsUnique();

        modelBuilder.Entity<LocationType>()
            .HasData(
                new LocationType { Id = 1, Name = "Care Unit" },
                new LocationType { Id = 2, Name = "Dementia Unit" },
                new LocationType { Id = 3, Name = "Village Unit" }
            );

        modelBuilder.Entity<BuildingBlock>()
            .Property(bb => bb.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<BuildingBlock>()
            .HasIndex(bb => new { bb.LocationTypeId, bb.Name })
            .IsUnique();

        modelBuilder.Entity<BuildingBlock>()
            .HasOne(bb => bb.LocationType)
            .WithMany(lt => lt.BuildingBlocks)
            .HasForeignKey(bb => bb.LocationTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Floor>()
            .Property(f => f.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Floor>()
            .HasIndex(f => new { f.BuildingBlockId, f.FloorNumber })
            .IsUnique();

        modelBuilder.Entity<Floor>()
            .HasOne(f => f.BuildingBlock)
            .WithMany(bb => bb.Floors)
            .HasForeignKey(f => f.BuildingBlockId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommonArea>()
            .Property(ca => ca.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<CommonArea>()
            .HasIndex(ca => new { ca.FloorId, ca.Name })
            .IsUnique();

        modelBuilder.Entity<CommonArea>()
            .HasOne(ca => ca.Floor)
            .WithMany(f => f.CommonAreas)
            .HasForeignKey(ca => ca.FloorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Unit>()
            .Property(u => u.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Unit>()
            .Property(u => u.UnitNumber)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Unit>()
            .HasIndex(u => new { u.FloorId, u.UnitNumber })
            .IsUnique();

        modelBuilder.Entity<Unit>()
            .HasOne(u => u.Floor)
            .WithMany(f => f.Units)
            .HasForeignKey(u => u.FloorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Apartment>()
            .Property(a => a.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Apartment>()
            .Property(a => a.ApartmentNumber)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Apartment>()
            .HasIndex(a => new { a.FloorId, a.ApartmentNumber })
            .IsUnique();

        modelBuilder.Entity<Apartment>()
            .HasOne(a => a.Floor)
            .WithMany(f => f.Apartments)
            .HasForeignKey(a => a.FloorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Resident>()
            .HasOne(r => r.Unit)
            .WithMany(u => u.Residents)
            .HasForeignKey(r => r.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Resident>()
            .HasOne(r => r.Apartment)
            .WithMany(a => a.Residents)
            .HasForeignKey(r => r.ApartmentId)
            .OnDelete(DeleteBehavior.Restrict);

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
