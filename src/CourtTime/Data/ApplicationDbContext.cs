using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CourtTime.Models;

namespace CourtTime.Data;

/// <summary>
/// EF Core context for CourtTime: ASP.NET Core Identity tables plus the
/// domain entities (Facility, Court, Booking, BlockedSlot).
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BlockedSlot> BlockedSlots => Set<BlockedSlot>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Identity configuration must run first.
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(user =>
        {
            user.Property(u => u.PreferredSport).HasConversion<string>().HasMaxLength(20);
        });

        builder.Entity<Facility>(facility =>
        {
            // Deleting a user must not silently delete their facility and its history.
            facility.HasOne(f => f.Manager)
                .WithMany(u => u.ManagedFacilities)
                .HasForeignKey(f => f.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Court>(court =>
        {
            court.Property(c => c.HourlyPrice).HasPrecision(10, 2);
            court.Property(c => c.Sport).HasConversion<string>().HasMaxLength(20);

            court.HasOne(c => c.Facility)
                .WithMany(f => f.Courts)
                .HasForeignKey(c => c.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Booking>(booking =>
        {
            booking.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
            booking.Property(b => b.PaymentStatus).HasConversion<string>().HasMaxLength(20);

            booking.HasOne(b => b.Court)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CourtId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict avoids multiple cascade paths in SQL Server (user -> facility -> court -> booking).
            booking.HasOne(b => b.Player)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Speeds up availability lookups: "bookings for court X on day Y".
            booking.HasIndex(b => new { b.CourtId, b.StartTime });

            booking.ToTable(t => t.HasCheckConstraint("CK_Booking_EndAfterStart", "[EndTime] > [StartTime]"));
        });

        builder.Entity<BlockedSlot>(slot =>
        {
            slot.Property(s => s.Reason).HasConversion<string>().HasMaxLength(20);

            slot.HasOne(s => s.Court)
                .WithMany(c => c.BlockedSlots)
                .HasForeignKey(s => s.CourtId)
                .OnDelete(DeleteBehavior.Cascade);

            slot.HasIndex(s => new { s.CourtId, s.StartTime });

            slot.ToTable(t => t.HasCheckConstraint("CK_BlockedSlot_EndAfterStart", "[EndTime] > [StartTime]"));
        });
    }
}
