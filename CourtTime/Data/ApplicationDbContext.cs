using CourtTime.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourtTime.Data;

/// <summary>
/// EF Core context for CourtTime. Inherits the ASP.NET Core Identity tables
/// (users, roles, claims) and adds the booking domain.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BlockedSlot> BlockedSlots => Set<BlockedSlot>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Facility>(entity =>
        {
            // Deleting a manager account must not silently delete the facility.
            entity.HasOne(f => f.Manager)
                .WithMany()
                .HasForeignKey(f => f.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Court>(entity =>
        {
            entity.Property(c => c.HourlyPrice).HasPrecision(10, 2);
            entity.Property(c => c.Sport).HasConversion<string>().HasMaxLength(20);
            entity.Property(c => c.Surface).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(c => c.Facility)
                .WithMany(f => f.Courts)
                .HasForeignKey(c => c.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Booking>(entity =>
        {
            entity.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(b => b.PaymentStatus).HasConversion<string>().HasMaxLength(20);

            // Availability queries always filter by court and time.
            entity.HasIndex(b => new { b.CourtId, b.StartTime });

            entity.HasOne(b => b.Court)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CourtId)
                .OnDelete(DeleteBehavior.Cascade);

            // Keep booking history even if a player account is removed later.
            entity.HasOne(b => b.Player)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<BlockedSlot>(entity =>
        {
            entity.Property(s => s.Reason).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(s => new { s.CourtId, s.StartTime });

            entity.HasOne(s => s.Court)
                .WithMany(c => c.BlockedSlots)
                .HasForeignKey(s => s.CourtId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
