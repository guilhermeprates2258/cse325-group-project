using System.Data;
using CourtTime.Data;
using CourtTime.Models;
using Microsoft.EntityFrameworkCore;

namespace CourtTime.Services;

/// <summary>
/// All booking rules live here so every page applies them the same way:
/// one-hour slots, inside opening hours, in the future, and never overlapping
/// another booking or a blocked slot.
/// </summary>
public class BookingService(ApplicationDbContext db, IClock clock)
{
    public static readonly TimeSpan SlotLength = TimeSpan.FromHours(1);

    /// <summary>Maximum number of days ahead a player can book.</summary>
    public const int BookingWindowDays = 30;

    /// <summary>Builds the hour-by-hour calendar of one court for one day.</summary>
    public async Task<List<TimeSlot>> GetDaySlotsAsync(Court court, DateOnly date, string? currentUserId)
    {
        var dayStart = date.ToDateTime(TimeOnly.MinValue);
        var dayEnd = dayStart.AddDays(1);

        var bookings = await db.Bookings
            .AsNoTracking()
            .Where(b => b.CourtId == court.Id
                        && b.Status != BookingStatus.Cancelled
                        && b.StartTime < dayEnd && b.EndTime > dayStart)
            .ToListAsync();

        var blocks = await db.BlockedSlots
            .AsNoTracking()
            .Where(s => s.CourtId == court.Id && s.StartTime < dayEnd && s.EndTime > dayStart)
            .ToListAsync();

        var now = clock.Now;
        var slots = new List<TimeSlot>();

        for (var start = date.ToDateTime(court.OpensAt);
             start + SlotLength <= date.ToDateTime(court.ClosesAt);
             start += SlotLength)
        {
            var end = start + SlotLength;
            var booking = bookings.FirstOrDefault(b => b.OverlapsWith(start, end));

            var state = start < now ? SlotState.Past
                : blocks.Any(s => s.OverlapsWith(start, end)) ? SlotState.Blocked
                : booking is not null ? SlotState.Booked
                : SlotState.Free;

            slots.Add(new TimeSlot(start, end, state,
                IsMine: booking is not null && booking.PlayerId == currentUserId));
        }

        return slots;
    }

    /// <summary>Creates a booking after checking every rule inside one serializable transaction.</summary>
    public async Task<BookingResult> CreateAsync(int courtId, string playerId, DateTime start)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var result = await ValidateNewBookingAsync(courtId, start, ignoreBookingId: null);
        if (!result.Succeeded)
        {
            return result;
        }

        db.Bookings.Add(new Booking
        {
            CourtId = courtId,
            PlayerId = playerId,
            StartTime = start,
            EndTime = start + SlotLength,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return BookingResult.Ok($"Court booked for {start:dddd, MMM d} at {start:HH:mm}.");
    }

    /// <summary>Moves an existing booking to a new slot: the new booking and the cancellation succeed or fail together.</summary>
    public async Task<BookingResult> RescheduleAsync(int bookingId, string playerId, int courtId, DateTime newStart)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var existing = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.PlayerId == playerId);
        if (existing is null || existing.Status != BookingStatus.Confirmed || existing.StartTime <= clock.Now)
        {
            return BookingResult.Fail("This booking can no longer be rescheduled.");
        }

        var result = await ValidateNewBookingAsync(courtId, newStart, ignoreBookingId: bookingId);
        if (!result.Succeeded)
        {
            return result;
        }

        existing.Status = BookingStatus.Cancelled;
        db.Bookings.Add(new Booking
        {
            CourtId = courtId,
            PlayerId = playerId,
            StartTime = newStart,
            EndTime = newStart + SlotLength,
            PaymentStatus = existing.PaymentStatus,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return BookingResult.Ok($"Booking moved to {newStart:dddd, MMM d} at {newStart:HH:mm}.");
    }

    /// <summary>Players can cancel their own confirmed bookings until the slot starts.</summary>
    public async Task<BookingResult> CancelAsync(int bookingId, string playerId)
    {
        var booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.PlayerId == playerId);
        if (booking is null)
        {
            return BookingResult.Fail("Booking not found.");
        }

        if (booking.Status != BookingStatus.Confirmed || booking.StartTime <= clock.Now)
        {
            return BookingResult.Fail("Only upcoming confirmed bookings can be cancelled.");
        }

        booking.Status = BookingStatus.Cancelled;
        await db.SaveChangesAsync();
        return BookingResult.Ok("Booking cancelled. The slot is now free for other players.");
    }

    private async Task<BookingResult> ValidateNewBookingAsync(int courtId, DateTime start, int? ignoreBookingId)
    {
        var court = await db.Courts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == courtId);
        if (court is null || !court.IsActive)
        {
            return BookingResult.Fail("This court is not available for booking.");
        }

        var end = start + SlotLength;
        var now = clock.Now;

        if (start <= now)
        {
            return BookingResult.Fail("You can only book future time slots.");
        }

        if (start > now.AddDays(BookingWindowDays))
        {
            return BookingResult.Fail($"Bookings open {BookingWindowDays} days in advance.");
        }

        if (start.Minute != 0 || start.Second != 0 || !court.IsOpenDuring(start, end))
        {
            return BookingResult.Fail("Please choose one of the time slots shown on the calendar.");
        }

        var blocked = await db.BlockedSlots.AnyAsync(s =>
            s.CourtId == courtId && s.StartTime < end && start < s.EndTime);
        if (blocked)
        {
            return BookingResult.Fail("This time is reserved by the facility.");
        }

        var taken = await db.Bookings.AnyAsync(b =>
            b.CourtId == courtId
            && b.Id != ignoreBookingId
            && b.Status != BookingStatus.Cancelled
            && b.StartTime < end && start < b.EndTime);
        if (taken)
        {
            return BookingResult.Fail("Sorry, someone just booked this slot. Please pick another time.");
        }

        return BookingResult.Ok(string.Empty);
    }
}
