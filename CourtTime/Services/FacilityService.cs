using CourtTime.Data;
using CourtTime.Models;
using Microsoft.EntityFrameworkCore;

namespace CourtTime.Services;

/// <summary>
/// Operations for Facility Managers. Every method takes the manager's user id and
/// only touches data that belongs to that manager's facility.
/// </summary>
public class FacilityService(ApplicationDbContext db)
{
    public Task<Facility?> GetFacilityAsync(string managerId) =>
        db.Facilities
            .Include(f => f.Courts)
            .FirstOrDefaultAsync(f => f.ManagerId == managerId);

    public async Task SaveFacilityAsync(string managerId, Facility input)
    {
        var facility = await db.Facilities.FirstOrDefaultAsync(f => f.ManagerId == managerId);
        if (facility is null)
        {
            facility = new Facility { ManagerId = managerId };
            db.Facilities.Add(facility);
        }

        facility.Name = input.Name.Trim();
        facility.Address = input.Address?.Trim();
        facility.ContactPhone = input.ContactPhone?.Trim();
        await db.SaveChangesAsync();
    }

    public Task<Court?> GetCourtAsync(string managerId, int courtId) =>
        db.Courts
            .Include(c => c.Facility)
            .FirstOrDefaultAsync(c => c.Id == courtId && c.Facility!.ManagerId == managerId);

    public async Task<bool> SaveCourtAsync(string managerId, Court input)
    {
        var facility = await db.Facilities.FirstOrDefaultAsync(f => f.ManagerId == managerId);
        if (facility is null)
        {
            return false;
        }

        Court? court;
        if (input.Id == 0)
        {
            court = new Court { FacilityId = facility.Id };
            db.Courts.Add(court);
        }
        else
        {
            court = await GetCourtAsync(managerId, input.Id);
            if (court is null)
            {
                return false;
            }
        }

        court.Name = input.Name.Trim();
        court.Sport = input.Sport;
        court.Surface = input.Surface;
        court.HourlyPrice = input.HourlyPrice;
        court.OpensAt = input.OpensAt;
        court.ClosesAt = input.ClosesAt;
        court.IsActive = input.IsActive;

        await db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Courts without booking history are deleted. Courts that have bookings are
    /// deactivated instead, so players' history stays intact.
    /// </summary>
    public async Task<string> DeleteCourtAsync(string managerId, int courtId)
    {
        var court = await GetCourtAsync(managerId, courtId);
        if (court is null)
        {
            return "Court not found.";
        }

        if (await db.Bookings.AnyAsync(b => b.CourtId == courtId))
        {
            court.IsActive = false;
            await db.SaveChangesAsync();
            return $"{court.Name} has booking history, so it was deactivated instead of deleted.";
        }

        db.Courts.Remove(court);
        await db.SaveChangesAsync();
        return $"{court.Name} was deleted.";
    }

    public Task<List<Booking>> GetScheduleAsync(string managerId, DateOnly date)
    {
        var dayStart = date.ToDateTime(TimeOnly.MinValue);
        var dayEnd = dayStart.AddDays(1);

        return db.Bookings
            .AsNoTracking()
            .Include(b => b.Court)
            .Include(b => b.Player)
            .Where(b => b.Court!.Facility!.ManagerId == managerId
                        && b.StartTime >= dayStart && b.StartTime < dayEnd)
            .OrderBy(b => b.StartTime)
            .ThenBy(b => b.Court!.Name)
            .ToListAsync();
    }

    public Task<List<BlockedSlot>> GetBlockedSlotsAsync(string managerId, DateOnly date)
    {
        var dayStart = date.ToDateTime(TimeOnly.MinValue);
        var dayEnd = dayStart.AddDays(1);

        return db.BlockedSlots
            .AsNoTracking()
            .Include(s => s.Court)
            .Where(s => s.Court!.Facility!.ManagerId == managerId
                        && s.StartTime < dayEnd && s.EndTime > dayStart)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    /// <summary>Applies a manager action to a booking: paid, unpaid, completed or no-show.</summary>
    public async Task<string> UpdateBookingAsync(string managerId, int bookingId, string action)
    {
        var booking = await db.Bookings
            .Include(b => b.Court)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.Court!.Facility!.ManagerId == managerId);
        if (booking is null)
        {
            return "Booking not found.";
        }

        switch (action)
        {
            case "paid":
                booking.PaymentStatus = PaymentStatus.Paid;
                break;
            case "unpaid":
                booking.PaymentStatus = PaymentStatus.Pending;
                break;
            case "completed":
                booking.Status = BookingStatus.Completed;
                break;
            case "noshow":
                booking.Status = BookingStatus.NoShow;
                break;
            default:
                return "Unknown action.";
        }

        await db.SaveChangesAsync();
        return $"Booking at {booking.StartTime:HH:mm} on {booking.Court?.Name} updated.";
    }

    public async Task<string> BlockSlotAsync(string managerId, BlockedSlot input)
    {
        var court = await GetCourtAsync(managerId, input.CourtId);
        if (court is null)
        {
            return "Error: court not found.";
        }

        if (input.EndTime <= input.StartTime)
        {
            return "Error: the block must end after it starts.";
        }

        var conflicts = await db.Bookings.CountAsync(b =>
            b.CourtId == input.CourtId
            && b.Status == BookingStatus.Confirmed
            && b.StartTime < input.EndTime && input.StartTime < b.EndTime);

        db.BlockedSlots.Add(new BlockedSlot
        {
            CourtId = input.CourtId,
            StartTime = input.StartTime,
            EndTime = input.EndTime,
            Reason = input.Reason,
            Notes = input.Notes?.Trim()
        });
        await db.SaveChangesAsync();

        return conflicts == 0
            ? $"{court.Name} blocked from {input.StartTime:HH:mm} to {input.EndTime:HH:mm}."
            : $"{court.Name} blocked. Note: {conflicts} existing booking(s) overlap this period; contact those players.";
    }

    public async Task<string> RemoveBlockAsync(string managerId, int blockId)
    {
        var block = await db.BlockedSlots
            .FirstOrDefaultAsync(s => s.Id == blockId && s.Court!.Facility!.ManagerId == managerId);
        if (block is null)
        {
            return "Error: blocked slot not found.";
        }

        db.BlockedSlots.Remove(block);
        await db.SaveChangesAsync();
        return "Blocked slot removed.";
    }
}
