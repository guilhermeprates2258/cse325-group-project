using System.ComponentModel.DataAnnotations;
using CourtTime.Data;

namespace CourtTime.Models;

/// <summary>A player's reservation of a court for a time period.</summary>
public class Booking : IValidatableObject
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "The booking must end after it starts.",
                [nameof(EndTime)]);
        }
    }

    /// <summary>
    /// Two periods overlap when each one starts before the other ends.
    /// Used by the booking feature to prevent double bookings.
    /// </summary>
    public bool OverlapsWith(DateTime start, DateTime end) =>
        Status != BookingStatus.Cancelled && StartTime < end && start < EndTime;
}
