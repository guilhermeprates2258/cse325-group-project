using System.ComponentModel.DataAnnotations;

namespace CourtTime.Models;

/// <summary>A bookable court. Opening hours and price are defined per court.</summary>
public class Court : IValidatableObject
{
    public int Id { get; set; }

    public int FacilityId { get; set; }
    public Facility? Facility { get; set; }

    [Required, StringLength(60)]
    public string Name { get; set; } = string.Empty;

    public SportType Sport { get; set; }

    public CourtSurface Surface { get; set; }

    [Range(0, 10000)]
    [DataType(DataType.Currency)]
    public decimal HourlyPrice { get; set; }

    public TimeOnly OpensAt { get; set; } = new(7, 0);

    public TimeOnly ClosesAt { get; set; } = new(23, 0);

    /// <summary>Inactive courts stay in the database (history) but cannot receive new bookings.</summary>
    public bool IsActive { get; set; } = true;

    public List<Booking> Bookings { get; set; } = [];
    public List<BlockedSlot> BlockedSlots { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ClosesAt <= OpensAt)
        {
            yield return new ValidationResult(
                "Closing time must be later than opening time.",
                [nameof(ClosesAt)]);
        }
    }

    /// <summary>True when the whole period falls inside the court's opening hours.</summary>
    public bool IsOpenDuring(DateTime start, DateTime end) =>
        start.Date == end.Date
        && TimeOnly.FromDateTime(start) >= OpensAt
        && TimeOnly.FromDateTime(end) <= ClosesAt;
}
