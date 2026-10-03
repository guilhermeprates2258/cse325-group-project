using System.ComponentModel.DataAnnotations;

namespace CourtTime.Models;

/// <summary>A bookable court inside a facility.</summary>
public class Court
{
    public int Id { get; set; }

    public int FacilityId { get; set; }
    public Facility? Facility { get; set; }

    [Required, StringLength(60)]
    public string Name { get; set; } = string.Empty;

    public SportType Sport { get; set; }

    [StringLength(40)]
    public string? Surface { get; set; }

    [Range(0, 10000)]
    public decimal HourlyPrice { get; set; }

    public TimeOnly OpeningTime { get; set; } = new(7, 0);
    public TimeOnly ClosingTime { get; set; } = new(23, 0);

    /// <summary>Inactive courts are hidden from players but keep their history.</summary>
    public bool IsActive { get; set; } = true;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<BlockedSlot> BlockedSlots { get; set; } = new List<BlockedSlot>();
}
