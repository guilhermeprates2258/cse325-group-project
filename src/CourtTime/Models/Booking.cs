using System.ComponentModel.DataAnnotations;
using CourtTime.Data;

namespace CourtTime.Models;

/// <summary>A player's reservation of a court for a time range.</summary>
public class Booking
{
    public int Id { get; set; }

    public int CourtId { get; set; }
    public Court? Court { get; set; }

    [Required]
    public string PlayerId { get; set; } = string.Empty;
    public ApplicationUser? Player { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// True when this booking overlaps the given range. Used together with a
    /// database transaction to prevent double bookings.
    /// </summary>
    public bool Overlaps(DateTime start, DateTime end) => start < EndTime && end > StartTime;
}
